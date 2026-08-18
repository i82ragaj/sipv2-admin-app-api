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

    public async Task<(IReadOnlyList<VparkingSummary> Items, int TotalCount)> GetPagedAsync(
        string? parkingId,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int pageIndex,
        int pageSize)
    {
        var query = _context.VparkingSummaries.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parkingId))
        {
            query = query.Where(s => s.Idpk == parkingId);
        }

        if (dateFrom.HasValue)
        {
            query = query.Where(s => s.Date >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(s => s.Date <= dateTo.Value);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.Date)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
