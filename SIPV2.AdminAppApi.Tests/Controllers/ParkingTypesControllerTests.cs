using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.ParkingTypes;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class ParkingTypesControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsTypesOrderedByName()
    {
        var repository = new FakeParkingTypeRepository();
        repository.Types.Add(new MdparkingType { Id = "SD", Name = "Skidata", Active = true });
        repository.Types.Add(new MdparkingType { Id = "EQ", Name = "Equinsa", Active = true });
        var controller = new ParkingTypesController(repository);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var types = Assert.IsAssignableFrom<IEnumerable<ParkingTypeDto>>(ok.Value).ToList();
        Assert.Equal(2, types.Count);
        Assert.Equal("Equinsa", types[0].Name);
        Assert.Equal("Skidata", types[1].Name);
    }
}
