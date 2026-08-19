using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeParkingSummaryDetailRepository : IParkingSummaryDetailRepository
{
    public List<VparkingSummaryDetail> Details { get; } = [];

    public Task<IReadOnlyList<VparkingSummaryDetail>> GetForSummaryAsync(
        string? summaryId,
        string idpk,
        DateOnly? date)
    {
        var query = !string.IsNullOrWhiteSpace(summaryId)
            ? Details.Where(d => d.Idsummary == summaryId)
            : Details.Where(d => d.Idpk == idpk && d.Date == date);

        IReadOnlyList<VparkingSummaryDetail> result = query.OrderBy(d => d.PaymentTypeName).ToList();
        return Task.FromResult(result);
    }
}
