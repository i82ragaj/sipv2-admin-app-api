using Microsoft.EntityFrameworkCore;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Services;

public class EfDailyTotalRepository : IDailyTotalRepository
{
    private readonly AppDbContext _context;

    public EfDailyTotalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(IReadOnlyList<VdailyTotal> Items, int TotalCount)> GetPagedAsync(
        string? parkingId,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int pageIndex,
        int pageSize)
    {
        var query = _context.VdailyTotals.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parkingId))
        {
            query = query.Where(t => t.Idpk == parkingId);
        }

        // TotalDate es DATETIME (con hora); dateFrom/dateTo llegan como fecha
        // sin hora, así que dateTo se lleva al final del día para incluirlo entero.
        if (dateFrom.HasValue)
        {
            var from = dateFrom.Value.ToDateTime(TimeOnly.MinValue);
            query = query.Where(t => t.TotalDate >= from);
        }

        if (dateTo.HasValue)
        {
            var to = dateTo.Value.ToDateTime(TimeOnly.MaxValue);
            query = query.Where(t => t.TotalDate <= to);
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.TotalDate)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<List<VdailyTotal>> GetSeriesAsync(string parkingId)
    {
        var lastAvailable = await _context.VdailyTotals
            .AsNoTracking()
            .Where(t => t.Idpk == parkingId)
            .OrderByDescending(t => t.TotalDate)
            .Select(t => (DateTime?)t.TotalDate)
            .FirstOrDefaultAsync();

        if (lastAvailable is null)
        {
            return [];
        }

        // Ventana de exactamente 7 días naturales (no 7 lecturas), con inicio
        // a las 00:00 del séptimo día contando hacia atrás desde el último
        // dato: si el último dato es el día D, la ventana es [D-6 00:00, D
        // último dato] = D-6..D, 7 días. Se ancla en la fecha del último
        // dato, no en "hoy", porque la importación puede llevar retraso.
        var from = lastAvailable.Value.Date.AddDays(-6);

        return await _context.VdailyTotals
            .AsNoTracking()
            .Where(t => t.Idpk == parkingId && t.TotalDate >= from && t.TotalDate <= lastAvailable.Value)
            .OrderBy(t => t.TotalDate)
            .ToListAsync();
    }
}
