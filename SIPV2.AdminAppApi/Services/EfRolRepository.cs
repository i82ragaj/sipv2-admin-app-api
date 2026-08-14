using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfRolRepository : IRolRepository
{
    private readonly AppDbContext _context;

    public EfRolRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Mdrol>> GetAllAsync() =>
        _context.Mdrols.AsNoTracking().ToListAsync();

    public Task<Mdrol?> GetByIdAsync(Guid id) =>
        _context.Mdrols.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Mdrol> CreateAsync(Mdrol rol)
    {
        rol.Id = Guid.NewGuid();
        rol.Active = true;
        rol.Created = DateTime.UtcNow;
        _context.Mdrols.Add(rol);
        await _context.SaveChangesAsync();
        return rol;
    }

    public async Task<bool> UpdateAsync(Mdrol rol)
    {
        var existing = await _context.Mdrols.FirstOrDefaultAsync(r => r.Id == rol.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Name = rol.Name;
        existing.Description = rol.Description;
        existing.Active = rol.Active;
        existing.Updated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Mdrols.FirstOrDefaultAsync(r => r.Id == id);
        if (existing is null)
        {
            return false;
        }

        _context.Mdrols.Remove(existing);
        await _context.SaveChangesAsync();
        return true;
    }
}
