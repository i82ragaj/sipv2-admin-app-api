# CLAUDE.md

Guía para trabajar con Claude Code en **sipv2-admin-app-api**. Lee primero
`ARCHITECTURE.md` (arquitectura general: capas, principios, flujo de auth) — este
archivo no lo repite, solo añade lo específico de este proyecto concreto y lo que hay
que recordar al hacer cambios.

## Qué es esto

API ASP.NET Core (.NET 10) para el panel de administración SIPV2: usuarios/roles,
catálogo de parkings/contadores, y endpoints de solo lectura para consulta (estado de
integración, informe ERP, ingresos diarios, ocupación actual). Consumida por el frontend
Angular del repo hermano `sipv2-admin-app` (repo separado, no submódulo) — antes de
cambiar la forma de un DTO, comprueba qué modelo usa el frontend para ese recurso, para
no romperlo en silencio.

Tres proyectos en `SIPV2.AdminAppApi.slnx`:
- `SIPV2.DataModels` — modelos EF Core, **100% generados por scaffold, nunca a mano**.
- `SIPV2.AdminAppApi` — la API en sí (Controllers/Services/Contracts).
- `SIPV2.AdminAppApi.Tests` — xUnit, controllers en proceso contra Fakes, sin DB real.

## Arrancar / compilar / testear

```bash
cd SIPV2.AdminAppApi
dotnet run --launch-profile https     # https://localhost:7119
```

Antes de recompilar o volver a arrancar, si ya había un proceso corriendo **mátalo
primero** (`taskkill //F //IM "SIPV2.AdminAppApi.exe"`): lo deja bloqueado
`SIPV2.DataModels.dll` y el build falla con `MSB3027`/`MSB3021`. Comprueba el estado real
del puerto (`netstat -ano | grep ":7119" | grep LISTENING`) en vez de suponerlo antes de
decidir si hay que arrancar o no.

```bash
dotnet test    # SIPV2.AdminAppApi.Tests — sin DB, no necesita ConnectionStrings real
```

Da por terminado un cambio en backend solo tras `dotnet build` limpio **y** `dotnet test`
en verde — el proyecto de tests no depende de infraestructura, así que no hay excusa para
saltárselo.

## Regenerar modelos desde la base de datos

`scaffold.ps1` en la raíz ejecuta `dotnet ef dbcontext scaffold --force` contra SQL
Server y regenera `SIPV2.DataModels/Models/` y `Data/AppDbContext.cs`. Es el **único**
modo sancionado de descubrir o cambiar la forma de una tabla/vista — nunca edites esos
archivos a mano, ni intentes inspeccionar el esquema con `sqlcmd`/consultas directas
cuando puedes correr este script (introspección de esquema, no requiere tocar datos).
El script trae credenciales de conexión hardcodeadas para desarrollo: si lo modificas,
no las subas a un commit compartido si cambian a algo sensible distinto del entorno de
dev ya versionado.

Tras cada scaffold, revisa el diff en `Models/` — puede cambiar la forma de una
`partial class` existente (p. ej. una nueva FK como `TypeNavigation` en `Mdparking` al
añadir `MDParkingType`) y hay que propagar el cambio a DTOs/repositorios a mano.

## Logging

Serilog (no el proveedor por defecto de ASP.NET Core), con el mismo patrón que
`sipv2-occupancy-api/SIPV2.OccupancyApi/Program.cs` — mantén ambos alineados si tocas uno:

- **Bootstrap logger** (`Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();`)
  antes de `WebApplication.CreateBuilder`, con **todo** `Program.cs` dentro de un
  `try { ... } catch (Exception ex) { Log.Fatal(...) } finally { Log.CloseAndFlush(); }`:
  un fallo durante el arranque (antes de que `UseSerilog` termine de configurarse) queda
  logueado igual, no se pierde.
- Niveles y sinks (consola + fichero) se controlan enteramente desde la sección `Serilog`
  de `appsettings.json`/`appsettings.Development.json` — ruta del fichero, rotación,
  retención — sin tocar código. Fichero en `logs/api-.log` (rota a diario, 14 días de
  retención, ruta relativa al content root). `logs/` está en `.gitignore`.
- **No se loguea cada request** (no hay `UseSerilogRequestLogging`): solo los accesos
  rechazados (401/403) via un middleware propio (`Log.Warning("Acceso no autorizado: ...")`),
  igual que en OccupancyApi — así el fichero no se llena de 200 rutinarios.
- Las excepciones no controladas *durante una request* (no en el arranque) las captura
  `Services/GlobalExceptionHandler.cs` (`IExceptionHandler`, registrado con
  `AddExceptionHandler<>()` + `app.UseExceptionHandler()` como middleware más externo de
  todos) — esto sí es propio de AdminApi, OccupancyApi no lo tiene: loguea la excepción
  completa (nivel `Error`, con traza) y responde al cliente un 500 genérico
  (`{"message": "Ha ocurrido un error inesperado."}`) sin filtrar el mensaje/stack trace real.

## Convenciones que hay que seguir SIEMPRE

- **Nunca `DbContext` directo en un controller.** Todo pasa por
  `I<Recurso>Repository` / `Ef<Recurso>Repository`. Al tocar un repositorio existente,
  el flujo completo es: interfaz → implementación EF → **`Fake<Recurso>Repository`
  hermano en Tests** (nada del compilador te avisa si lo olvidas, solo la interfaz
  compartida) → tests del controller.
- **DTOs en `Contracts/<Recurso>/`, nunca entidades EF expuestas**, aunque el mapeo sea
  1:1. Un controller nuevo sigue siempre el mismo esqueleto que `ParkingsController`:
  `GetAll`/`GetById`/`Create`/`Update`/`Delete` (o el subconjunto que aplique) +
  `ToDto(entidad)` privado al final del archivo.
- **`[Authorize(Roles = "...")]` no es opcional ni aditivo al fallback.** `Program.cs`
  define un `FallbackPolicy` que exige rol `admin` para cualquier endpoint sin
  `[Authorize]`/`[AllowAnonymous]` explícito — pero **en cuanto un controller pone
  cualquier `[Authorize]`, sale de ese fallback** y debe repetir *todos* sus roles
  permitidos a mano, incluido `admin` si el admin también debe poder usarlo (patrón real
  en el código: `[Authorize(Roles = "admin,config")]`, `"admin,status"`, etc.). Olvidar
  `admin` en la lista es un bug de regresión típico al copiar un controller existente.
  Los nombres de rol se guardan **en minúsculas** por convención (`admin`, `security`,
  `config`, `status`, `apiweb`) — no los captialices ni inventes uno nuevo sin mirar
  `MDRol` en BD primero.
- **Borrado lógico.** "Eliminar" = `Active = false` vía UPDATE, no un DELETE físico,
  salvo un caso documentado (`UserRolesController.Update` solo permite tocar `Active`;
  el frontend hace DELETE real solo si el rol nunca tuvo asignación activa). No añadas
  un DELETE físico nuevo sin confirmar que es realmente ese caso.
- **Catálogos con tabla propia** (p. ej. `MDParkingType`): repositorio de solo lectura
  (`GetAllAsync()` ordenado por nombre) + DTO simple + controller `[HttpGet]` único; no
  hardcodees valores que ya tengan tabla en BD, y no le añadas Create/Update/Delete si
  el frontend no los necesita — sigue el alcance real pedido, no lo que "podría hacer
  falta".
- **Endpoints de solo lectura sobre vistas `[Keyless]`** (`ParkingStatuses`,
  `ParkingSummaries`, `DailyTotals`/`VDailyTotal`, `Occupancy`/`VOccupationActual`): sin
  Create/Update/Delete, y sus Fakes en tests son simples listas en memoria — nunca
  intentes sembrarlos vía EF (`Add`/`SaveChanges` no funciona sobre `[Keyless]` en
  ningún proveedor), por eso los tests usan Fakes y no `EF Core InMemory`.
- **Config falla pronto.** `appsettings.json` trae placeholders vacíos a propósito
  (`ConnectionStrings:DefaultConnection`, `Jwt:Key`); `Program.cs` lanza excepción si
  faltan al arrancar. Los valores reales de desarrollo van solo en
  `appsettings.Development.json` (no versionar secretos reales fuera de ahí).
- **Ventanas de fechas / agregados temporales**: cuidado con los off-by-one al calcular
  "últimos N días" (`AddDays(-6)` para una ventana de exactamente 7 días incluyendo hoy,
  no `-7`); si añades un endpoint con lógica de rango de fechas, añade un test de borde
  explícito para el límite inicial/final, no solo el caso feliz.

## Estructura (resumen; detalle completo en ARCHITECTURE.md)

```
SIPV2.DataModels/Models/            generado por scaffold — no editar
SIPV2.AdminAppApi/
  Controllers/                      un controller por recurso, [Authorize(Roles=...)] explícito
  Services/I<R>Repository.cs        interfaz — lo que ve el controller
  Services/Ef<R>Repository.cs       implementación real sobre AppDbContext
  Contracts/<Recurso>/              DTOs de request/response
SIPV2.AdminAppApi.Tests/
  Fakes/Fake<R>Repository.cs        List<T> en memoria, misma interfaz que Ef<R>Repository
  Controllers/<R>ControllerTests.cs
```

Al añadir un recurso nuevo: DTO(s) en `Contracts/<Recurso>/`, `I<Recurso>Repository` +
`Ef<Recurso>Repository` en `Services/`, registro en `Program.cs`
(`AddScoped<I...,Ef...>`), controller en `Controllers/` con roles explícitos,
`Fake<Recurso>Repository` en `Tests/Fakes/`, tests en `Tests/Controllers/`. Confirma con
el frontend qué campos necesita antes de decidir la forma del DTO.

## Repo hermano

El frontend vive en `sipv2-admin-app` (repo separado). Antes de cambiar el nombre/forma
de un campo en un DTO, revisa el modelo TypeScript correspondiente en
`src/app/core/models/` de ese repo — deben reflejarse uno a uno.
