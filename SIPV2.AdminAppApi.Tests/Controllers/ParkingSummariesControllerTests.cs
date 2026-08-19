using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts;
using SIPV2.AdminAppApi.Contracts.ParkingSummaries;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class ParkingSummariesControllerTests
{
    private static ParkingSummariesController CreateController(
        FakeParkingSummaryRepository? repository = null,
        FakeParkingSummaryDetailRepository? detailRepository = null) =>
        new(repository ?? new FakeParkingSummaryRepository(), detailRepository ?? new FakeParkingSummaryDetailRepository());

    [Fact]
    public async Task GetAll_ReturnsAllSummaries()
    {
        var repository = new FakeParkingSummaryRepository();
        repository.Summaries.Add(new VparkingSummary { Idpk = "PK01", Total = 100.5m });
        var controller = CreateController(repository);

        var result = await controller.GetAll(null, null, null, 0, 10);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<ParkingSummaryDto>>(ok.Value);
        Assert.Single(page.Items);
        Assert.Equal(1, page.TotalCount);
    }

    [Fact]
    public async Task GetAll_WhenEmpty_ReturnsEmptyList()
    {
        var controller = CreateController();

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
        var controller = CreateController(repository);

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
        var controller = CreateController(repository);

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
        var controller = CreateController(repository);

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

    [Fact]
    public async Task GetDetails_FiltersBySummaryIdWhenProvided()
    {
        var detailRepository = new FakeParkingSummaryDetailRepository();
        detailRepository.Details.Add(new VparkingSummaryDetail
        {
            Idsummary = "SUM1",
            Idpk = "PK01",
            Date = new DateOnly(2025, 8, 1),
            PaymentTypeName = "Contado/Efectivo",
            Operations = 10,
            Total = 100m,
        });
        detailRepository.Details.Add(new VparkingSummaryDetail
        {
            Idsummary = "SUM2",
            Idpk = "PK01",
            Date = new DateOnly(2025, 8, 1),
            PaymentTypeName = "Tarjeta (EMV)",
            Operations = 5,
            Total = 50m,
        });
        var controller = CreateController(detailRepository: detailRepository);

        var result = await controller.GetDetails("PK01", new DateOnly(2025, 8, 1), "SUM1");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var details = Assert.IsAssignableFrom<IEnumerable<ParkingSummaryDetailDto>>(ok.Value);
        var detail = Assert.Single(details);
        Assert.Equal("Contado/Efectivo", detail.PaymentTypeName);
    }

    [Fact]
    public async Task GetDetails_FallsBackToIdpkAndDateWhenSummaryIdMissing()
    {
        var detailRepository = new FakeParkingSummaryDetailRepository();
        detailRepository.Details.Add(new VparkingSummaryDetail
        {
            Idpk = "PK01",
            Date = new DateOnly(2025, 8, 1),
            PaymentTypeName = "Contado/Efectivo",
            Operations = 10,
            Total = 100m,
        });
        detailRepository.Details.Add(new VparkingSummaryDetail
        {
            Idpk = "PK02",
            Date = new DateOnly(2025, 8, 1),
            PaymentTypeName = "Tarjeta (EMV)",
            Operations = 5,
            Total = 50m,
        });
        var controller = CreateController(detailRepository: detailRepository);

        var result = await controller.GetDetails("PK01", new DateOnly(2025, 8, 1), null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var details = Assert.IsAssignableFrom<IEnumerable<ParkingSummaryDetailDto>>(ok.Value);
        var detail = Assert.Single(details);
        Assert.Equal("Contado/Efectivo", detail.PaymentTypeName);
    }
}
