using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.UserRoles;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

[ApiController]
[Route("api/user-roles")]
[Authorize(Roles = "admin")]
public class UserRolesController : ControllerBase
{
    private readonly IUserRolRepository _repository;

    public UserRolesController(IUserRolRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<UserRolDto>>> GetAll()
    {
        var userRoles = await _repository.GetAllAsync();
        return Ok(userRoles.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserRolDto>> GetById(Guid id)
    {
        var userRol = await _repository.GetByIdAsync(id);
        return userRol is null ? NotFound() : Ok(ToDto(userRol));
    }

    [HttpPost]
    public async Task<ActionResult<UserRolDto>> Create([FromBody] CreateUserRolRequest request)
    {
        var existing = await _repository.GetByUserAndRolAsync(request.UserId, request.RolId);
        if (existing is not null)
        {
            return Conflict(new { message = "Ese usuario ya tiene asignado ese rol." });
        }

        var userRol = new MduserRol
        {
            UserId = request.UserId,
            RolId = request.RolId,
        };

        var created = await _repository.CreateAsync(userRol);
        var withNavigations = await _repository.GetByIdAsync(created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(withNavigations ?? created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateActive(Guid id, [FromBody] UpdateUserRolRequest request)
    {
        var updated = await _repository.UpdateActiveAsync(id, request.Active);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    private static UserRolDto ToDto(MduserRol userRol) => new()
    {
        Id = userRol.Id,
        UserId = userRol.UserId,
        UserLogin = userRol.User?.Login,
        RolId = userRol.RolId,
        RolName = userRol.Rol?.Name,
        Active = userRol.Active,
    };
}
