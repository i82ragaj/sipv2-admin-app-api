using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.UserRoles;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class UserRolesControllerTests
{
    [Fact]
    public async Task Create_AssignsRoleToUser()
    {
        var controller = new UserRolesController(new FakeUserRolRepository());
        var userId = Guid.NewGuid();
        var rolId = Guid.NewGuid();

        var result = await controller.Create(new CreateUserRolRequest { UserId = userId, RolId = rolId });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<UserRolDto>(created.Value);
        Assert.Equal(userId, dto.UserId);
        Assert.Equal(rolId, dto.RolId);
        Assert.True(dto.Active);
    }

    [Fact]
    public async Task Create_WhenAlreadyAssigned_ReturnsConflict()
    {
        var repository = new FakeUserRolRepository();
        var userId = Guid.NewGuid();
        var rolId = Guid.NewGuid();
        repository.UserRoles.Add(new MduserRol { Id = Guid.NewGuid(), UserId = userId, RolId = rolId, Active = true });
        var controller = new UserRolesController(repository);

        var result = await controller.Create(new CreateUserRolRequest { UserId = userId, RolId = rolId });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task UpdateActive_TogglesAssignment()
    {
        var repository = new FakeUserRolRepository();
        var id = Guid.NewGuid();
        repository.UserRoles.Add(new MduserRol { Id = id, UserId = Guid.NewGuid(), RolId = Guid.NewGuid(), Active = true });
        var controller = new UserRolesController(repository);

        var result = await controller.UpdateActive(id, new UpdateUserRolRequest { Active = false });

        Assert.IsType<NoContentResult>(result);
        Assert.False(repository.UserRoles.Single().Active);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        var controller = new UserRolesController(new FakeUserRolRepository());

        var result = await controller.Delete(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }
}
