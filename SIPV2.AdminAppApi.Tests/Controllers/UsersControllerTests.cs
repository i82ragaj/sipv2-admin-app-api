using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Users;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class UsersControllerTests
{
    [Fact]
    public async Task GetAll_ReturnsAllUsers()
    {
        var repository = new FakeUserRepository();
        repository.Users.Add(new Mduser { Id = Guid.NewGuid(), Name = "Ana", Login = "ana", Active = true });
        var controller = new UsersController(repository);

        var result = await controller.GetAll();

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsAssignableFrom<IEnumerable<UserDto>>(ok.Value);
        Assert.Single(users);
    }

    [Fact]
    public async Task GetById_WhenMissing_ReturnsNotFound()
    {
        var controller = new UsersController(new FakeUserRepository());

        var result = await controller.GetById(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Create_WithNewLogin_ReturnsCreated()
    {
        var controller = new UsersController(new FakeUserRepository());

        var result = await controller.Create(new CreateUserRequest
        {
            Name = "Ana",
            Login = "ana",
            Password = "Secreta123",
        });

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var dto = Assert.IsType<UserDto>(created.Value);
        Assert.Equal("ana", dto.Login);
        Assert.True(dto.Active);
    }

    [Fact]
    public async Task Create_WithDuplicateLogin_ReturnsConflict()
    {
        var repository = new FakeUserRepository();
        repository.Users.Add(new Mduser { Id = Guid.NewGuid(), Name = "Ana", Login = "ana", Active = true });
        var controller = new UsersController(repository);

        var result = await controller.Create(new CreateUserRequest { Name = "Otra", Login = "ana", Password = "Secreta123" });

        Assert.IsType<ConflictObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_WhenExists_ReturnsNoContent()
    {
        var repository = new FakeUserRepository();
        var id = Guid.NewGuid();
        repository.Users.Add(new Mduser { Id = id, Name = "Ana", Login = "ana", Active = true });
        var controller = new UsersController(repository);

        var result = await controller.Update(id, new UpdateUserRequest { Name = "Ana María", Active = false });

        Assert.IsType<NoContentResult>(result);
        Assert.Equal("Ana María", repository.Users.Single().Name);
        Assert.False(repository.Users.Single().Active);
    }

    [Fact]
    public async Task Delete_WhenMissing_ReturnsNotFound()
    {
        var controller = new UsersController(new FakeUserRepository());

        var result = await controller.Delete(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ResetPassword_WhenExists_UpdatesHashAndReturnsNoContent()
    {
        var repository = new FakeUserRepository();
        var id = Guid.NewGuid();
        var originalHash = BCrypt.Net.BCrypt.HashPassword("Original123");
        repository.Users.Add(new Mduser { Id = id, Name = "Ana", Login = "ana", Active = true, Password = originalHash });
        var controller = new UsersController(repository);

        var result = await controller.ResetPassword(id, new ResetPasswordRequest { NewPassword = "NuevaClave123" });

        Assert.IsType<NoContentResult>(result);
        var updatedHash = repository.Users.Single().Password;
        Assert.NotEqual(originalHash, updatedHash);
        Assert.True(BCrypt.Net.BCrypt.Verify("NuevaClave123", updatedHash));
    }

    [Fact]
    public async Task ResetPassword_WhenMissing_ReturnsNotFound()
    {
        var controller = new UsersController(new FakeUserRepository());

        var result = await controller.ResetPassword(Guid.NewGuid(), new ResetPasswordRequest { NewPassword = "NuevaClave123" });

        Assert.IsType<NotFoundResult>(result);
    }
}
