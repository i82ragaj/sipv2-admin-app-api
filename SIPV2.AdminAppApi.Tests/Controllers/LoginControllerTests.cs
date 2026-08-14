using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Auth;
using SIPV2.AdminAppApi.Controllers;
using SIPV2.AdminAppApi.Tests.Fakes;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Tests.Controllers;

public class LoginControllerTests
{
    private static (LoginController controller, FakeAuthRepository authRepository) CreateController()
    {
        var authRepository = new FakeAuthRepository();
        var jwtTokenService = new FakeJwtTokenService();
        var controller = new LoginController(authRepository, jwtTokenService);
        return (controller, authRepository);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokenAndRoles()
    {
        var (controller, authRepository) = CreateController();
        var userId = Guid.NewGuid();
        var rolId = Guid.NewGuid();
        authRepository.Users.Add(new Mduser
        {
            Id = userId,
            Login = "jperez",
            Name = "Juan",
            Password = BCrypt.Net.BCrypt.HashPassword("Secreta123"),
            Active = true,
        });
        authRepository.Roles.Add(new Mdrol { Id = rolId, Name = "Admin", Active = true });
        authRepository.UserRoles.Add(new MduserRol { UserId = userId, RolId = rolId, Active = true });

        var result = await controller.Login(new LoginRequest { Login = "jperez", Password = "Secreta123" });

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.Equal(userId, response.UserId);
        Assert.Contains("Admin", response.Roles);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        var (controller, authRepository) = CreateController();
        authRepository.Users.Add(new Mduser
        {
            Id = Guid.NewGuid(),
            Login = "jperez",
            Name = "Juan",
            Password = BCrypt.Net.BCrypt.HashPassword("Secreta123"),
            Active = true,
        });

        var result = await controller.Login(new LoginRequest { Login = "jperez", Password = "incorrecta" });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_WithUnknownLogin_ReturnsUnauthorized()
    {
        var (controller, _) = CreateController();

        var result = await controller.Login(new LoginRequest { Login = "no-existe", Password = "cualquiera" });

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }
}
