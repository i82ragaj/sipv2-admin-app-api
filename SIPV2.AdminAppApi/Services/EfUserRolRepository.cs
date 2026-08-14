using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfUserRolRepository : IUserRolRepository
{
    private readonly AppDbContext _context;

    public EfUserRolRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<MduserRol>> GetAllAsync() =>
        _context.MduserRols.AsNoTracking().Include(ur => ur.User).Include(ur => ur.Rol).ToListAsync();

    public Task<MduserRol?> GetByIdAsync(Guid id) =>
        _context.MduserRols.AsNoTracking().Include(ur => ur.User).Include(ur => ur.Rol)
            .FirstOrDefaultAsync(ur => ur.Id == id);

    public Task<MduserRol?> GetByUserAndRolAsync(Guid userId, Guid rolId) =>
        _context.MduserRols.AsNoTracking()
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RolId == rolId);

    public async Task<MduserRol> CreateAsync(MduserRol userRol)
    {
        userRol.Id = Guid.NewGuid();
        userRol.Active = true;
        userRol.Created = DateTime.UtcNow;
        _context.MduserRols.Add(userRol);
        await _context.SaveChangesAsync();
        return userRol;
    }

    public async Task<bool> UpdateActiveAsync(Guid id, bool active)
    {
        var existing = await _context.MduserRols.FirstOrDefaultAsync(ur => ur.Id == id);
        if (existing is null)
        {
            return false;
        }

        existing.Active = active;
        existing.Updated = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    // "Eliminar" es borrado lógico en toda la aplicación: se desactiva la
    // asignación (Active = false) en vez de borrar la fila físicamente.
    public Task<bool> DeleteAsync(Guid id) => UpdateActiveAsync(id, false);
}
