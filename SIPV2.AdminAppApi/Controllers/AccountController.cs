using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Users;
using SIPV2.AdminAppApi.Services;

namespace SIPV2.AdminAppApi.Controllers;

// Autoservicio para el usuario autenticado (cualquier rol, no solo Admin): actúa sobre
// su propia cuenta a partir del token, nunca sobre un Id recibido del cliente.
[ApiController]
[Route("api/account")]
[Authorize]
public class AccountController : ControllerBase
{
    private readonly IUserRepository _repository;

    public AccountController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangeOwnPassword([FromBody] ChangeOwnPasswordRequest request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _repository.GetByIdAsync(userId);
        if (user is null || user.Password is null || !BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.Password))
        {
            return BadRequest(new { message = "La contraseña actual no es correcta." });
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _repository.UpdatePasswordAsync(userId, passwordHash);
        return NoContent();
    }
}
