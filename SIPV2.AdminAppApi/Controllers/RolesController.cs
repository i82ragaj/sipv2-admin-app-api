using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Roles;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

[ApiController]
[Route("api/roles")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IRolRepository _repository;

    public RolesController(IRolRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<RolDto>>> GetAll()
    {
        var roles = await _repository.GetAllAsync();
        return Ok(roles.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<RolDto>> GetById(Guid id)
    {
        var rol = await _repository.GetByIdAsync(id);
        return rol is null ? NotFound() : Ok(ToDto(rol));
    }

    [HttpPost]
    public async Task<ActionResult<RolDto>> Create([FromBody] CreateRolRequest request)
    {
        var rol = new Mdrol
        {
            Name = request.Name,
            Description = request.Description,
        };

        var created = await _repository.CreateAsync(rol);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRolRequest request)
    {
        var rol = new Mdrol
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Active = request.Active,
        };

        var updated = await _repository.UpdateAsync(rol);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    private static RolDto ToDto(Mdrol rol) => new()
    {
        Id = rol.Id,
        Name = rol.Name,
        Description = rol.Description,
        Active = rol.Active,
    };
}
