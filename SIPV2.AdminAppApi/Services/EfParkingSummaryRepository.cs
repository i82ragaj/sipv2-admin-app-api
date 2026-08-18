using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfParkingSummaryRepository : IParkingSummaryRepository
{
    private readonly AppDbContext _context;

    public EfParkingSummaryRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<VparkingSummary>> GetAllAsync() =>
        _context.VparkingSummaries.AsNoTracking().ToListAsync();
}
