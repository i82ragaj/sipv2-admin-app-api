using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

// VParkingSummary es una vista de solo lectura (sin clave, [Keyless]): no hay
// Create/Update/Delete, solo consulta.
public interface IParkingSummaryRepository
{
    // Filtrado (parkingId/dateFrom/dateTo) y paginación resueltos en servidor;
    // siempre ordenado por fecha descendente (más actuales primero).
    Task<(IReadOnlyList<VparkingSummary> Items, int TotalCount)> GetPagedAsync(
        string? parkingId,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int pageIndex,
        int pageSize);
}
