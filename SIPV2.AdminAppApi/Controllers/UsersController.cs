using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Users;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

[ApiController]
[Route("api/users")]
// Grupo "Seguridad" del menú (Usuarios/Roles): rol "security", o "admin" (ve todo).
[Authorize(Roles = "admin,security")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repository;

    public UsersController(IUserRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _repository.GetAllAsync();
        return Ok(users.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(ToDto(user));
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserRequest request)
    {
        var existing = await _repository.GetByLoginAsync(request.Login);
        if (existing is not null)
        {
            return Conflict(new { message = "Ya existe un usuario con ese login." });
        }

        var user = new Mduser
        {
            Name = request.Name,
            LastName = request.LastName,
            LastName1 = request.LastName1,
            Login = request.Login,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email,
            Phone = request.Phone,
            Active = true,
        };

        var created = await _repository.CreateAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        var user = new Mduser
        {
            Id = id,
            Name = request.Name,
            LastName = request.LastName,
            LastName1 = request.LastName1,
            Email = request.Email,
            Phone = request.Phone,
            Active = request.Active,
        };

        var updated = await _repository.UpdateAsync(user);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    // Un Admin fija la contraseña de cualquier usuario (no requiere conocer la actual).
    [HttpPut("{id:guid}/password")]
    public async Task<IActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequest request)
    {
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        var updated = await _repository.UpdatePasswordAsync(id, passwordHash);
        return updated ? NoContent() : NotFound();
    }

    private static UserDto ToDto(Mduser user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        LastName = user.LastName,
        LastName1 = user.LastName1,
        Login = user.Login,
        Email = user.Email,
        Phone = user.Phone,
        Active = user.Active,
    };
}
