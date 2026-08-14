using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Auth;
using SIPV2.AdminAppApi.Services;

namespace SIPV2.AdminAppApi.Controllers;

[ApiController]
[AllowAnonymous]
public class LoginController : ControllerBase
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginController(IAuthRepository authRepository, IJwtTokenService jwtTokenService)
    {
        _authRepository = authRepository;
        _jwtTokenService = jwtTokenService;
    }

    [HttpPost("/login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _authRepository.GetActiveUserByLoginAsync(request.Login);
        if (user is null || user.Password is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
        {
            return Unauthorized(new { message = "Usuario o contraseña incorrectos." });
        }

        var roles = await _authRepository.GetRoleNamesAsync(user.Id);
        var token = _jwtTokenService.GenerateToken(user, roles);

        return Ok(new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Roles = roles,
        });
    }
}
