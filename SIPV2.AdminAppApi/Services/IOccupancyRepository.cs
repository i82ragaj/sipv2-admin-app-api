using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

// VOccupationActual es una vista de solo lectura (sin clave, [Keyless]): no hay
// Create/Update/Delete, solo consulta. Sin paginación: es una foto del estado
// actual, no un histórico, así que el volumen es el de contadores activos.
public interface IOccupancyRepository
{
    Task<List<VoccupationActual>> GetCurrentAsync(string? parkingId, string? counterName);
}
