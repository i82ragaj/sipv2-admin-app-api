using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeParkingSummaryRepository : IParkingSummaryRepository
{
    public List<VparkingSummary> Summaries { get; } = [];

    public Task<(IReadOnlyList<VparkingSummary> Items, int TotalCount)> GetPagedAsync(
        string? parkingId,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int pageIndex,
        int pageSize)
    {
        var query = Summaries.AsEnumerable();

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

        var filtered = query.OrderByDescending(s => s.Date).ToList();
        var page = filtered.Skip(pageIndex * pageSize).Take(pageSize).ToList();

        return Task.FromResult<(IReadOnlyList<VparkingSummary>, int)>((page, filtered.Count));
    }
}
