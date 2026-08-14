using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

// Solo lectura: MDParkingStatus lo mantiene el proceso de importación, no esta API.
public interface IParkingStatusRepository
{
    Task<List<MdparkingStatus>> GetAllAsync();

    Task<MdparkingStatus?> GetByIdAsync(string id);
}
