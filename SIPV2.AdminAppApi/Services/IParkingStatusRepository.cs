using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public enum RequestDailyImportResult
{
    NotFound,

    // El último estado no era OK ni ERROR (p. ej. ya está PENDIENTE): no se
    // vuelve a solicitar para no pisar una importación ya en curso.
    NotAllowed,

    Success,
}

// MDParkingStatus lo mantiene el proceso de importación, no esta API; la
// única escritura permitida desde aquí es solicitar una importación diaria
// (RequestDailyImportAsync), que solo marca el estado como PENDIENTE.
public interface IParkingStatusRepository
{
    Task<List<MdparkingStatus>> GetAllAsync();

    Task<MdparkingStatus?> GetByIdAsync(string id);

    Task<RequestDailyImportResult> RequestDailyImportAsync(string id);
}
