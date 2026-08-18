using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts;
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

        var result = await controller.GetAll(null, null, null, 0, 10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<ParkingSummaryDto>>(ok.Value);
        Assert.Single(page.Items);
        Assert.Equal(1, page.TotalCount);
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ReturnsEmptyList()
    {
        var controller = new ParkingSummariesController(new FakeParkingSummaryRepository());

        var result = await controller.GetAll(null, null, null, 0, 10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<ParkingSummaryDto>>(ok.Value);
        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalCount);
    }

    [Fact]
    public async Task GetAll_FiltersByParkingId()
    {
        var repository = new FakeParkingSummaryRepository();
        repository.Summaries.Add(new VparkingSummary { Idpk = "PK01", Total = 1m });
        repository.Summaries.Add(new VparkingSummary { Idpk = "PK02", Total = 2m });
        var controller = new ParkingSummariesController(repository);

        var result = await controller.GetAll("PK02", null, null, 0, 10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<ParkingSummaryDto>>(ok.Value);
        var summary = Assert.Single(page.Items);
        Assert.Equal("PK02", summary.Idpk);
        Assert.Equal(1, page.TotalCount);
    }

    [Fact]
    public async Task GetAll_FiltersByDateRangeAndOrdersDescending()
    {
        var repository = new FakeParkingSummaryRepository();
        repository.Summaries.Add(new VparkingSummary { Idpk = "PK01", Date = new DateOnly(2025, 8, 1) });
        repository.Summaries.Add(new VparkingSummary { Idpk = "PK01", Date = new DateOnly(2025, 8, 3) });
        repository.Summaries.Add(new VparkingSummary { Idpk = "PK01", Date = new DateOnly(2025, 8, 5) });
        var controller = new ParkingSummariesController(repository);

        var result = await controller.GetAll(null, new DateOnly(2025, 8, 2), new DateOnly(2025, 8, 4), 0, 10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<ParkingSummaryDto>>(ok.Value);
        var summary = Assert.Single(page.Items);
        Assert.Equal(new DateOnly(2025, 8, 3), summary.Date);
    }

    [Fact]
    public async Task GetAll_PaginatesResults()
    {
        var repository = new FakeParkingSummaryRepository();
        for (var day = 1; day <= 5; day++)
        {
            repository.Summaries.Add(new VparkingSummary { Idpk = "PK01", Date = new DateOnly(2025, 8, day) });
        }
        var controller = new ParkingSummariesController(repository);

        var firstPage = await controller.GetAll(null, null, null, 0, 2);
        var firstOk = Assert.IsType<OkObjectResult>(firstPage.Result);
        var firstResult = Assert.IsType<PagedResult<ParkingSummaryDto>>(firstOk.Value);

        Assert.Equal(5, firstResult.TotalCount);
        Assert.Equal(2, firstResult.Items.Count);
        // Más actuales primero.
        Assert.Equal(new DateOnly(2025, 8, 5), firstResult.Items[0].Date);
        Assert.Equal(new DateOnly(2025, 8, 4), firstResult.Items[1].Date);

        var secondPage = await controller.GetAll(null, null, null, 1, 2);
        var secondOk = Assert.IsType<OkObjectResult>(secondPage.Result);
        var secondResult = Assert.IsType<PagedResult<ParkingSummaryDto>>(secondOk.Value);

        Assert.Equal(2, secondResult.Items.Count);
        Assert.Equal(new DateOnly(2025, 8, 3), secondResult.Items[0].Date);
        Assert.Equal(new DateOnly(2025, 8, 2), secondResult.Items[1].Date);
    }
}
