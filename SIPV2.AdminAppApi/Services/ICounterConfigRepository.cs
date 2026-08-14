using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public interface ICounterConfigRepository
{
    Task<List<MdcounterConfig>> GetAllAsync();

    Task<MdcounterConfig?> GetByIdAsync(Guid id);

    Task<MdcounterConfig?> GetByIdpkAndCounterIdAsync(string idpk, string counterId);

    Task<MdcounterConfig> CreateAsync(MdcounterConfig config);

    Task<bool> UpdateAsync(MdcounterConfig config);

    Task<bool> DeleteAsync(Guid id);
}
