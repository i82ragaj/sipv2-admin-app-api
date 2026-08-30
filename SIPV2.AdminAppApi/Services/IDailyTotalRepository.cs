using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

// VDailyTotal es una vista de solo lectura (sin clave, [Keyless]): no hay
// Create/Update/Delete, solo consulta.
public interface IDailyTotalRepository
{
    // Filtrado (parkingId/dateFrom/dateTo) y paginación resueltos en servidor;
    // siempre ordenado por fecha descendente (más actuales primero).
    Task<(IReadOnlyList<VdailyTotal> Items, int TotalCount)> GetPagedAsync(
        string? parkingId,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int pageIndex,
        int pageSize);

    // Serie sin paginar para el gráfico de evolución: todas las lecturas de un
    // parking desde 7 días antes de su último dato disponible hasta ese
    // último dato, ordenadas ascendente. Vacía si el parking no tiene lecturas.
    Task<List<VdailyTotal>> GetSeriesAsync(string parkingId);
}
