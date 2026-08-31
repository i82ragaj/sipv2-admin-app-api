using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

// Bootstrap logger: cubre errores durante el arranque, antes de que builder.Host.UseSerilog()
// configure el logger definitivo (con los sinks/niveles de la sección "Serilog" de appsettings).
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Niveles y sinks (consola, fichero: ruta, rotación, retención...) se configuran
    // enteramente desde la sección "Serilog" de appsettings*.json -- ver ese fichero
    // para cambiarlos sin tocar código. Aquí solo se añade el enriquecimiento común.
    builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
        .Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration));

    // --- Config: falla pronto si falta algo crítico ---
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException("Falta configurar 'ConnectionStrings:DefaultConnection'.");
    }

    var jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrWhiteSpace(jwtKey))
    {
        throw new InvalidOperationException("Falta configurar 'Jwt:Key'.");
    }

    var jwtIssuer = builder.Configuration["Jwt:Issuer"];
    var jwtAudience = builder.Configuration["Jwt:Audience"];

    // --- DbContext (EF Core sobre SIPV2.DataModels) ---
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

    // --- Repositorios ---
    builder.Services.AddScoped<IAuthRepository, EfAuthRepository>();
    builder.Services.AddScoped<IUserRepository, EfUserRepository>();
    builder.Services.AddScoped<IRolRepository, EfRolRepository>();
    builder.Services.AddScoped<IUserRolRepository, EfUserRolRepository>();
    builder.Services.AddScoped<IParkingRepository, EfParkingRepository>();
    builder.Services.AddScoped<IParkingTypeRepository, EfParkingTypeRepository>();
    builder.Services.AddScoped<IParkingStatusRepository, EfParkingStatusRepository>();
    builder.Services.AddScoped<IParkingSummaryRepository, EfParkingSummaryRepository>();
    builder.Services.AddScoped<IParkingSummaryDetailRepository, EfParkingSummaryDetailRepository>();
    builder.Services.AddScoped<ICounterConfigRepository, EfCounterConfigRepository>();
    builder.Services.AddScoped<IDailyTotalRepository, EfDailyTotalRepository>();
    builder.Services.AddScoped<IOccupancyRepository, EfOccupancyRepository>();

    // --- Servicios de infraestructura ---
    builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

    // --- Auth JWT + rol por defecto (secure by default) ---
    builder.Services
        .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = !string.IsNullOrWhiteSpace(jwtIssuer),
                ValidIssuer = jwtIssuer,
                ValidateAudience = !string.IsNullOrWhiteSpace(jwtAudience),
                ValidAudience = jwtAudience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            };
        });

    builder.Services.AddAuthorization(options =>
    {
        // Cualquier endpoint sin [Authorize]/[AllowAnonymous] explícito queda protegido igualmente.
        // Convención: los nombres de rol se guardan en minúsculas (ver Roles en MDRol).
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("admin")
            .Build();
    });

    // Captura cualquier excepción no controlada: la loguea con traza completa
    // (GlobalExceptionHandler) en vez de dejarla reventar hasta el cliente.
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "SIPV2 Admin App API", Version = "v1" });

        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Introduce el token JWT como: Bearer {token}",
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
        };
        options.AddSecurityDefinition("Bearer", securityScheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement { { securityScheme, Array.Empty<string>() } });
    });

    var app = builder.Build();

    // Middleware más externo: envuelve todo lo demás para que ninguna excepción
    // no controlada escape sin loguearse (ver GlobalExceptionHandler).
    app.UseExceptionHandler();

    // Swagger siempre en Development (aunque alguien ponga Swagger:Enabled=false en un
    // appsettings compartido); en el resto de entornos, "Swagger:Enabled" manda -- por
    // defecto true (a diferencia de SIPV2.OccupancyApi) para poder consultarlo ya
    // desplegado; ponlo a false en el appsettings del servidor si no quieres exponerlo.
    if (app.Environment.IsDevelopment() || builder.Configuration.GetValue("Swagger:Enabled", true))
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    // Log de eventos de negocio: accesos rechazados por falta/insuficiencia de autenticación o rol.
    app.Use(async (context, next) =>
    {
        await next();

        if (context.Response.StatusCode is StatusCodes.Status401Unauthorized or StatusCodes.Status403Forbidden)
        {
            Log.Warning(
                "Acceso no autorizado: {Method} {Path} -> {StatusCode}",
                context.Request.Method, context.Request.Path, context.Response.StatusCode);
        }
    });

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "SIPV2.AdminAppApi terminó de forma inesperada durante el arranque");
}
finally
{
    Log.CloseAndFlush();
}
