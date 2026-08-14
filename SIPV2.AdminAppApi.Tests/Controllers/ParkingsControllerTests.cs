using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Parkings;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class ParkingsControllerTests
{
    [Fact]
    public async Task Create_WithNewId_ReturnsCreated()
    {
        var controller = new ParkingsController(new FakeParkingRepository());

        var result = await controller.Create(new CreateParkingRequest { Id = "PK01", Type = "N" });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<ParkingDto>(created.Value);
        Assert.Equal("PK01", dto.Id);
        Assert.True(dto.Active);
    }

    [Fact]
    public async Task Create_WithDuplicateId_ReturnsConflict()
    {
        var repository = new FakeParkingRepository();
        repository.Parkings.Add(new Mdparking { Id = "PK01", Type = "N", Active = true });
        var controller = new ParkingsController(repository);

        var result = await controller.Create(new CreateParkingRequest { Id = "PK01", Type = "N" });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFound()
    {
        var controller = new ParkingsController(new FakeParkingRepository());

        var result = await controller.GetById("no-existe");

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_WhenExists_UpdatesFields()
    {
        var repository = new FakeParkingRepository();
        repository.Parkings.Add(new Mdparking { Id = "PK01", Type = "N", Active = true });
        var controller = new ParkingsController(repository);

        var result = await controller.Update("PK01", new UpdateParkingRequest { Type = "M", Active = false, Name = "Nuevo nombre" });

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("M", repository.Parkings.Single().Type);
        Assert.Equal("Nuevo nombre", repository.Parkings.Single().Name);
        Assert.False(repository.Parkings.Single().Active);
    }
}
