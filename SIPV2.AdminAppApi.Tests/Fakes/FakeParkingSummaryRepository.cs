using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Fakes;

public class FakeParkingSummaryRepository : IParkingSummaryRepository
{
    public List<VparkingSummary> Summaries { get; } = [];

    public Task<List<VparkingSummary>> GetAllAsync() => Task.FromResult(Summaries.ToList());
}
