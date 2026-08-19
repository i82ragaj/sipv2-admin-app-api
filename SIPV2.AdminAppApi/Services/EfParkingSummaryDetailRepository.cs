using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfParkingSummaryDetailRepository : IParkingSummaryDetailRepository
{
    private readonly AppDbContext _context;

    public EfParkingSummaryDetailRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<VparkingSummaryDetail>> GetForSummaryAsync(
        string? summaryId,
        string idpk,
        DateOnly? date)
    {
        var query = _context.VparkingSummaryDetails.AsNoTracking().AsQueryable();

        query = !string.IsNullOrWhiteSpace(summaryId)
            ? query.Where(d => d.Idsummary == summaryId)
            : query.Where(d => d.Idpk == idpk && d.Date == date);

        return await query.OrderBy(d => d.PaymentTypeName).ToListAsync();
    }
}
