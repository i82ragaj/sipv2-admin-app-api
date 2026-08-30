using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts;
using SIPV2.AdminAppApi.Contracts.DailyTotals;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class DailyTotalsControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsPagedResult()
    {
        var repository = new FakeDailyTotalRepository();
        repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = new DateTime(2026, 1, 1), DailyTotalAmount = 123.45m });
        var controller = new DailyTotalsController(repository);

        var result = await controller.GetAll(null, null, null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<DailyTotalDto>>(ok.Value);
        Assert.Single(page.Items);
        Assert.Equal(1, page.TotalCount);
        Assert.Equal("PK01", page.Items[0].Idpk);
    }

    [Fact]
    public async Task GetAll_FiltersByParkingId()
    {
        var repository = new FakeDailyTotalRepository();
        repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = new DateTime(2026, 1, 1) });
        repository.Totals.Add(new VdailyTotal { Idpk = "PK02", TotalDate = new DateTime(2026, 1, 1) });
        var controller = new DailyTotalsController(repository);

        var result = await controller.GetAll("PK02", null, null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<DailyTotalDto>>(ok.Value);
        Assert.Single(page.Items);
        Assert.Equal("PK02", page.Items[0].Idpk);
    }

    [Fact]
    public async Task GetAll_ClampsPageSize()
    {
        var repository = new FakeDailyTotalRepository();
        for (var i = 0; i < 5; i++)
        {
            repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = new DateTime(2026, 1, 1).AddDays(i) });
        }
        var controller = new DailyTotalsController(repository);

        var result = await controller.GetAll(null, null, null, pageIndex: 0, pageSize: 1000);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<PagedResult<DailyTotalDto>>(ok.Value);
        Assert.Equal(5, page.Items.Count);
    }

    [Fact]
    public async Task GetSeries_WithoutParkingId_ReturnsBadRequest()
    {
        var controller = new DailyTotalsController(new FakeDailyTotalRepository());

        var result = await controller.GetSeries(null!);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetSeries_ReturnsLast7DaysBeforeLastAvailableReading()
    {
        var repository = new FakeDailyTotalRepository();
        var lastAvailable = new DateTime(2026, 1, 10, 18, 0, 0);
        repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = lastAvailable.AddDays(-10), DailyTotalAmount = 1 }); // fuera de rango
        repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = lastAvailable.AddDays(-3), DailyTotalAmount = 50 });
        repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = lastAvailable, DailyTotalAmount = 100 });
        repository.Totals.Add(new VdailyTotal { Idpk = "PK02", TotalDate = lastAvailable, DailyTotalAmount = 999 }); // otro parking
        var controller = new DailyTotalsController(repository);

        var result = await controller.GetSeries("PK01");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var series = Assert.IsAssignableFrom<IEnumerable<DailyTotalDto>>(ok.Value).ToList();
        Assert.Equal(2, series.Count);
        Assert.All(series, s => Assert.Equal("PK01", s.Idpk));
        // Ascendente por fecha.
        Assert.True(series[0].TotalDate < series[1].TotalDate);
    }

    [Fact]
    public async Task GetSeries_WindowIsExactly7DaysStartingAtMidnightOfTheSeventhDayBack()
    {
        var repository = new FakeDailyTotalRepository();
        var lastAvailable = new DateTime(2026, 1, 10, 18, 0, 0);
        // Justo dentro: 00:00 del séptimo día contando hacia atrás (D-6).
        var boundaryIn = lastAvailable.Date.AddDays(-6);
        // Justo fuera: el instante inmediatamente anterior (día D-7, 23:59:59).
        var boundaryOut = boundaryIn.AddTicks(-1);

        repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = boundaryOut, DailyTotalAmount = 1 });
        repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = boundaryIn, DailyTotalAmount = 2 });
        repository.Totals.Add(new VdailyTotal { Idpk = "PK01", TotalDate = lastAvailable, DailyTotalAmount = 3 });
        var controller = new DailyTotalsController(repository);

        var result = await controller.GetSeries("PK01");

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var series = Assert.IsAssignableFrom<IEnumerable<DailyTotalDto>>(ok.Value).ToList();
        Assert.Equal(2, series.Count);
        Assert.DoesNotContain(series, s => s.TotalDate == boundaryOut);
        Assert.Contains(series, s => s.TotalDate == boundaryIn);
    }
}
