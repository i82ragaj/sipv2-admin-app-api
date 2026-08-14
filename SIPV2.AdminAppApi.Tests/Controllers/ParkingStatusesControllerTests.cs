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
}
