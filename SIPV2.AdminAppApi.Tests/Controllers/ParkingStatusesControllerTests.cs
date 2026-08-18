using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.ParkingStatuses;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class ParkingStatusesControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsAllStatuses()
    {
        var repository = new FakeParkingStatusRepository();
        repository.Statuses.Add(new MdparkingStatus { Id = "PK01", Active = true });
        var controller = new ParkingStatusesController(repository);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var statuses = Assert.IsAssignableFrom<IEnumerable<ParkingStatusDto>>(ok.Value);
        Assert.Single(statuses);
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFound()
    {
        var controller = new ParkingStatusesController(new FakeParkingStatusRepository());

        var result = await controller.GetById("no-existe");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task RequestDailyImport_WhenMissing_ReturnsNotFound()
    {
        var controller = new ParkingStatusesController(new FakeParkingStatusRepository());

        var result = await controller.RequestDailyImport("no-existe");

        Assert.IsType<NotFoundResult>(result);
    }

    [Theory]
    [InlineData("OK")]
    [InlineData("ERROR")]
    [InlineData(null)]
    public async Task RequestDailyImport_WhenOkErrorOrNull_SetsStatusPending(string? currentStatus)
    {
        var repository = new FakeParkingStatusRepository();
        repository.Statuses.Add(new MdparkingStatus { Id = "PK01", Active = true, LastImportedStatus = currentStatus });
        var controller = new ParkingStatusesController(repository);

        var result = await controller.RequestDailyImport("PK01");

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("PENDIENTE", repository.Statuses.Single().LastImportedStatus);
    }

    [Fact]
    public async Task RequestDailyImport_WhenAlreadyPending_ReturnsConflict()
    {
        var repository = new FakeParkingStatusRepository();
        repository.Statuses.Add(new MdparkingStatus { Id = "PK01", Active = true, LastImportedStatus = "PENDIENTE" });
        var controller = new ParkingStatusesController(repository);

        var result = await controller.RequestDailyImport("PK01");

        Assert.IsType<ConflictObjectResult>(result);
    }
}
