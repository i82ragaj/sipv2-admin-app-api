using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Occupancy;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class OccupancyControllerTests
{
    [Fact]
    public async Task GetCurrent_ReturnsAllOccupancy()
    {
        var repository = new FakeOccupancyRepository();
        repository.Occupancy.Add(new VoccupationActual
        {
            ParkingId = "PK01",
            ParkingName = "Parking 1",
            CounterId = "PK01_C1",
            CounterCode = "C1",
            CounterName = "Todos",
            CurrentLevel = 10,
        });
        var controller = new OccupancyController(repository);

        var result = await controller.GetCurrent(null, null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var occupancy = Assert.IsAssignableFrom<IEnumerable<CurrentOccupancyDto>>(ok.Value);
        Assert.Single(occupancy);
    }

    [Fact]
    public async Task GetCurrent_FiltersByParkingId()
    {
        var repository = new FakeOccupancyRepository();
        repository.Occupancy.Add(new VoccupationActual { ParkingId = "PK01", CounterId = "PK01_C1", CounterCode = "C1", CounterName = "Todos", CurrentLevel = 5 });
        repository.Occupancy.Add(new VoccupationActual { ParkingId = "PK02", CounterId = "PK02_C1", CounterCode = "C1", CounterName = "Todos", CurrentLevel = 7 });
        var controller = new OccupancyController(repository);

        var result = await controller.GetCurrent("PK02", null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var occupancy = Assert.IsAssignableFrom<IEnumerable<CurrentOccupancyDto>>(ok.Value).ToList();
        Assert.Single(occupancy);
        Assert.Equal("PK02", occupancy[0].ParkingId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Entrada")]
    public async Task GetCurrent_ExcludesCountersWithoutTodosInName(string? counterName)
    {
        var repository = new FakeOccupancyRepository();
        repository.Occupancy.Add(new VoccupationActual { ParkingId = "PK01", CounterId = "PK01_C1", CounterCode = "C1", CounterName = counterName, CurrentLevel = 5 });
        repository.Occupancy.Add(new VoccupationActual { ParkingId = "PK01", CounterId = "PK01_C2", CounterCode = "C2", CounterName = "Todos", CurrentLevel = 3 });
        var controller = new OccupancyController(repository);

        var result = await controller.GetCurrent(null, null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var occupancy = Assert.IsAssignableFrom<IEnumerable<CurrentOccupancyDto>>(ok.Value).ToList();
        Assert.Single(occupancy);
        Assert.Equal("C2", occupancy[0].CounterCode);
    }

    [Theory]
    [InlineData("Todos")]
    [InlineData("TODOS")]
    [InlineData("Todos Planta 1")]
    [InlineData("Plaza Todos")]
    public async Task GetCurrent_MatchesTodosCaseInsensitiveAndAsSubstring(string counterName)
    {
        var repository = new FakeOccupancyRepository();
        repository.Occupancy.Add(new VoccupationActual { ParkingId = "PK01", CounterId = "PK01_C1", CounterCode = "C1", CounterName = counterName, CurrentLevel = 5 });
        var controller = new OccupancyController(repository);

        var result = await controller.GetCurrent(null, null);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var occupancy = Assert.IsAssignableFrom<IEnumerable<CurrentOccupancyDto>>(ok.Value);
        Assert.Single(occupancy);
    }
}
