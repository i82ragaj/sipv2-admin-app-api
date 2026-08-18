using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.ParkingSummaries;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class ParkingSummariesControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsAllSummaries()
    {
        var repository = new FakeParkingSummaryRepository();
        repository.Summaries.Add(new VparkingSummary { Idpk = "PK01", Total = 100.5m });
        var controller = new ParkingSummariesController(repository);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var summaries = Assert.IsAssignableFrom<IEnumerable<ParkingSummaryDto>>(ok.Value);
        Assert.Single(summaries);
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ReturnsEmptyList()
    {
        var controller = new ParkingSummariesController(new FakeParkingSummaryRepository());

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var summaries = Assert.IsAssignableFrom<IEnumerable<ParkingSummaryDto>>(ok.Value);
        Assert.Empty(summaries);
    }
}
