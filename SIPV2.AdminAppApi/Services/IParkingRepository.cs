using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public interface IParkingRepository
{
    Task<List<Mdparking>> GetAllAsync();

    Task<Mdparking?> GetByIdAsync(string id);

    Task<Mdparking> CreateAsync(Mdparking parking);

    Task<bool> UpdateAsync(Mdparking parking);

    Task<bool> DeleteAsync(string id);
}
