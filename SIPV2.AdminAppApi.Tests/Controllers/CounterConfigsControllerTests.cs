using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.CounterConfigs;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class CounterConfigsControllerTests
{
    [Fact]
    public async Task Create_WithNewCombination_ReturnsCreated()
    {
        var controller = new CounterConfigsController(new FakeCounterConfigRepository());

        var result = await controller.Create(new CreateCounterConfigRequest { Idpk = "PK01", CounterId = "C1" });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<CounterConfigDto>(created.Value);
        Assert.Equal("PK01", dto.Idpk);
        Assert.True(dto.IsActive);
    }

    [Fact]
    public async Task Create_WithDuplicateCombination_ReturnsConflict()
    {
        var repository = new FakeCounterConfigRepository();
        repository.Configs.Add(new MdcounterConfig { Id = Guid.NewGuid(), Idpk = "PK01", CounterId = "C1", IsActive = true });
        var controller = new CounterConfigsController(repository);

        var result = await controller.Create(new CreateCounterConfigRequest { Idpk = "PK01", CounterId = "C1" });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Delete_WhenExists_RemovesConfig()
    {
        var repository = new FakeCounterConfigRepository();
        var id = Guid.NewGuid();
        repository.Configs.Add(new MdcounterConfig { Id = id, Idpk = "PK01", CounterId = "C1", IsActive = true });
        var controller = new CounterConfigsController(repository);

        var result = await controller.Delete(id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(repository.Configs);
    }
}
