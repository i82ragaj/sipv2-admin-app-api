using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Roles;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class RolesControllerTests
{
    [Fact]
    public async Task Create_AddsActiveRole()
    {
        var controller = new RolesController(new FakeRolRepository());

        var result = await controller.Create(new CreateRolRequest { Name = "Admin", Description = "Administrador" });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<RolDto>(created.Value);
        Assert.True(dto.Active);
        Assert.Equal("Admin", dto.Name);
    }

    [Fact]
    public async Task GetAll_ReturnsAllRoles()
    {
        var repository = new FakeRolRepository();
        repository.Roles.Add(new Mdrol { Id = Guid.NewGuid(), Name = "Admin", Active = true });
        var controller = new RolesController(repository);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var roles = Assert.IsAssignableFrom<IEnumerable<RolDto>>(ok.Value);
        Assert.Single(roles);
    }

    [Fact]
    public async Task Update_WhenMissing_ReturnsNotFound()
    {
        var controller = new RolesController(new FakeRolRepository());

        var result = await controller.Update(Guid.NewGuid(), new UpdateRolRequest { Name = "X", Active = true });

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_WhenExists_ReturnsNoContent()
    {
        var repository = new FakeRolRepository();
        var id = Guid.NewGuid();
        repository.Roles.Add(new Mdrol { Id = id, Name = "Admin", Active = true });
        var controller = new RolesController(repository);

        var result = await controller.Delete(id);

        Assert.IsType<NoContentResult>(result);
        Assert.Empty(repository.Roles);
    }
}
