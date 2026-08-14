using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeCounterConfigRepository : ICounterConfigRepository
{
    public List<MdcounterConfig> Configs { get; } = [];

    public Task<List<MdcounterConfig>> GetAllAsync() => Task.FromResult(Configs.ToList());

    public Task<MdcounterConfig?> GetByIdAsync(Guid id) => Task.FromResult(Configs.FirstOrDefault(c => c.Id == id));

    public Task<MdcounterConfig?> GetByIdpkAndCounterIdAsync(string idpk, string counterId) =>
        Task.FromResult(Configs.FirstOrDefault(c => c.Idpk == idpk && c.CounterId == counterId));

    public Task<MdcounterConfig> CreateAsync(MdcounterConfig config)
    {
        config.Id = Guid.NewGuid();
        config.IsActive = true;
        Configs.Add(config);
        return Task.FromResult(config);
    }

    public Task<bool> UpdateAsync(MdcounterConfig config)
    {
        var existing = Configs.FirstOrDefault(c => c.Id == config.Id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        existing.CounterName = config.CounterName;
        existing.OccupancyLimit = config.OccupancyLimit;
        existing.CounterType = config.CounterType;
        existing.IsActive = config.IsActive;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(Guid id)
    {
        var existing = Configs.FirstOrDefault(c => c.Id == id);
        if (existing is null)
        {
            return Task.FromResult(false);
        }

        Configs.Remove(existing);
        return Task.FromResult(true);
    }
}
