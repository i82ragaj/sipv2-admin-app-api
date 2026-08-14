using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfCounterConfigRepository : ICounterConfigRepository
{
    private readonly AppDbContext _context;

    public EfCounterConfigRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<MdcounterConfig>> GetAllAsync() =>
        _context.MdcounterConfigs.AsNoTracking().ToListAsync();

    public Task<MdcounterConfig?> GetByIdAsync(Guid id) =>
        _context.MdcounterConfigs.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

    public Task<MdcounterConfig?> GetByIdpkAndCounterIdAsync(string idpk, string counterId) =>
        _context.MdcounterConfigs.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Idpk == idpk && c.CounterId == counterId);

    public async Task<MdcounterConfig> CreateAsync(MdcounterConfig config)
    {
        config.Id = Guid.NewGuid();
        config.IsActive = true;
        config.Created = DateTime.UtcNow;
        _context.MdcounterConfigs.Add(config);
        await _context.SaveChangesAsync();
        return config;
    }

    public async Task<bool> UpdateAsync(MdcounterConfig config)
    {
        var existing = await _context.MdcounterConfigs.FirstOrDefaultAsync(c => c.Id == config.Id);
        if (existing is null)
        {
            return false;
        }

        existing.CounterName = config.CounterName;
        existing.OccupancyLimit = config.OccupancyLimit;
        existing.CounterType = config.CounterType;
        existing.IsActive = config.IsActive;
        existing.Updated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.MdcounterConfigs.FirstOrDefaultAsync(c => c.Id == id);
        if (existing is null)
        {
            return false;
        }

        _context.MdcounterConfigs.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
