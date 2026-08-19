using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

// VParkingSummaryDetail es una vista de solo lectura (sin clave, [Keyless]):
// solo consulta, desglosada por tipo de pago para una fila de VParkingSummary.
public interface IParkingSummaryDetailRepository
{
    // Filtra por Idsummary (Id de la fila padre en VParkingSummary) cuando se
    // conoce; si no (Id nulo en el padre), cae a idpk+fecha.
    Task<IReadOnlyList<VparkingSummaryDetail>> GetForSummaryAsync(
        string? summaryId,
        string idpk,
        DateOnly? date);
}
