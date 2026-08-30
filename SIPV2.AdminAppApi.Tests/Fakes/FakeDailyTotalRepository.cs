using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeDailyTotalRepository : IDailyTotalRepository
{
    public List<VdailyTotal> Totals { get; } = [];

    public Task<(IReadOnlyList<VdailyTotal> Items, int TotalCount)> GetPagedAsync(
        string? parkingId,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int pageIndex,
        int pageSize)
    {
        var query = Totals.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(parkingId))
        {
            query = query.Where(t => t.Idpk == parkingId);
        }

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

        var filtered = query.OrderByDescending(t => t.TotalDate).ToList();
        var page = filtered.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        return Task.FromResult<(IReadOnlyList<VdailyTotal>, int)>((page, filtered.Count));
    }

    public Task<List<VdailyTotal>> GetSeriesAsync(string parkingId)
    {
        var forParking = Totals.Where(t => t.Idpk == parkingId).ToList();
        if (forParking.Count == 0)
        {
            return Task.FromResult(new List<VdailyTotal>());
        }

        var lastAvailable = forParking.Max(t => t.TotalDate);
        var from = lastAvailable.Date.AddDays(-6); // ver EfDailyTotalRepository: 7 días exactos

        return Task.FromResult(
            forParking.Where(t => t.TotalDate >= from && t.TotalDate <= lastAvailable)
                .OrderBy(t => t.TotalDate)
                .ToList());
    }
}
