using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfParkingStatusRepository : IParkingStatusRepository
{
    private readonly AppDbContext _context;

    public EfParkingStatusRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<MdparkingStatus>> GetAllAsync() =>
        _context.MdparkingStatuses.AsNoTracking().ToListAsync();

    public Task<MdparkingStatus?> GetByIdAsync(string id) =>
        _context.MdparkingStatuses.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
}
