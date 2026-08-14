using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfAuthRepository : IAuthRepository
{
    private readonly AppDbContext _context;

    public EfAuthRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Mduser?> GetActiveUserByLoginAsync(string login) =>
        _context.Mdusers.AsNoTracking().FirstOrDefaultAsync(u => u.Login == login && u.Active);

    public Task<List<string>> GetRoleNamesAsync(Guid userId) =>
        _context.MduserRols
            .AsNoTracking()
            .Where(ur => ur.UserId == userId && ur.Active && ur.Rol != null && ur.Rol.Active)
            .Select(ur => ur.Rol!.Name!)
            .Where(name => name != null)
            .ToListAsync();
}
