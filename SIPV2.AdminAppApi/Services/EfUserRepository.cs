using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfUserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public EfUserRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Mduser>> GetAllAsync() =>
        _context.Mdusers.AsNoTracking().ToListAsync();

    public Task<Mduser?> GetByIdAsync(Guid id) =>
        _context.Mdusers.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public Task<Mduser?> GetByLoginAsync(string login) =>
        _context.Mdusers.AsNoTracking().FirstOrDefaultAsync(u => u.Login == login);

    public async Task<Mduser> CreateAsync(Mduser user)
    {
        user.Id = Guid.NewGuid();
        user.Created = DateTime.UtcNow;
        _context.Mdusers.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<bool> UpdateAsync(Mduser user)
    {
        var existing = await _context.Mdusers.FirstOrDefaultAsync(u => u.Id == user.Id);
        if (existing is null)
        {
            return false;
        }

        existing.Name = user.Name;
        existing.LastName = user.LastName;
        existing.LastName1 = user.LastName1;
        existing.Email = user.Email;
        existing.Phone = user.Phone;
        existing.Active = user.Active;
        existing.Updated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdatePasswordAsync(Guid id, string passwordHash)
    {
        var existing = await _context.Mdusers.FirstOrDefaultAsync(u => u.Id == id);
        if (existing is null)
        {
            return false;
        }

        existing.Password = passwordHash;
        existing.Updated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // "Eliminar" es borrado lógico en toda la aplicación: se desactiva el
    // registro (Active = false) en vez de borrar la fila físicamente.
    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await _context.Mdusers.FirstOrDefaultAsync(u => u.Id == id);
        if (existing is null)
        {
            return false;
        }

        existing.Active = false;
        existing.Updated = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
