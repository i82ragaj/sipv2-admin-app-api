using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Users;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class AccountControllerTests
{
    private static AccountController CreateController(FakeUserRepository repository, Guid userId)
    {
        var identity = new ClaimsIdentity([new Claim(ClaimTypes.NameIdentifier, userId.ToString())], "TestAuth");
        return new AccountController(repository)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = new ClaimsPrincipal(identity) },
            },
        };
    }

    [Fact]
    public async Task ChangeOwnPassword_WithCorrectCurrentPassword_UpdatesHashAndReturnsNoContent()
    {
        var repository = new FakeUserRepository();
        var id = Guid.NewGuid();
        var originalHash = BCrypt.Net.BCrypt.HashPassword("Original123");
        repository.Users.Add(new Mduser { Id = id, Name = "Ana", Login = "ana", Active = true, Password = originalHash });
        var controller = CreateController(repository, id);

        var result = await controller.ChangeOwnPassword(new ChangeOwnPasswordRequest
        {
            CurrentPassword = "Original123",
            NewPassword = "NuevaClave123",
        });

        Assert.IsType<NoContentResult>(result);
        var updatedHash = repository.Users.Single().Password;
        Assert.True(BCrypt.Net.BCrypt.Verify("NuevaClave123", updatedHash));
    }

    [Fact]
    public async Task ChangeOwnPassword_WithWrongCurrentPassword_ReturnsBadRequestAndDoesNotChangeHash()
    {
        var repository = new FakeUserRepository();
        var id = Guid.NewGuid();
        var originalHash = BCrypt.Net.BCrypt.HashPassword("Original123");
        repository.Users.Add(new Mduser { Id = id, Name = "Ana", Login = "ana", Active = true, Password = originalHash });
        var controller = CreateController(repository, id);

        var result = await controller.ChangeOwnPassword(new ChangeOwnPasswordRequest
        {
            CurrentPassword = "Incorrecta",
            NewPassword = "NuevaClave123",
        });

        Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal(originalHash, repository.Users.Single().Password);
    }

    [Fact]
    public async Task ChangeOwnPassword_OnlyAffectsTheAuthenticatedUsersOwnAccount()
    {
        var repository = new FakeUserRepository();
        var ownId = Guid.NewGuid();
        var otherId = Guid.NewGuid();
        var ownHash = BCrypt.Net.BCrypt.HashPassword("Original123");
        var otherHash = BCrypt.Net.BCrypt.HashPassword("OtraClave123");
        repository.Users.Add(new Mduser { Id = ownId, Name = "Ana", Login = "ana", Active = true, Password = ownHash });
        repository.Users.Add(new Mduser { Id = otherId, Name = "Beto", Login = "beto", Active = true, Password = otherHash });
        var controller = CreateController(repository, ownId);

        await controller.ChangeOwnPassword(new ChangeOwnPasswordRequest
        {
            CurrentPassword = "Original123",
            NewPassword = "NuevaClave123",
        });

        Assert.Equal(otherHash, repository.Users.Single(u => u.Id == otherId).Password);
    }
}
