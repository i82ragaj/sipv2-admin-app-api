using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfParkingTypeRepository : IParkingTypeRepository
{
    private readonly AppDbContext _context;

    public EfParkingTypeRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<MdparkingType>> GetAllAsync() =>
        _context.MdparkingTypes.AsNoTracking().OrderBy(t => t.Name).ToListAsync();
}
