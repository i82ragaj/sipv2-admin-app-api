using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

// VParkingSummary es una vista de solo lectura (sin clave, [Keyless]): no hay
// Create/Update/Delete, solo consulta.
public interface IParkingSummaryRepository
{
    Task<List<VparkingSummary>> GetAllAsync();
}
