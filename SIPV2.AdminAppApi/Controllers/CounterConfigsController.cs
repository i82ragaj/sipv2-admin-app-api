using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.CounterConfigs;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

[ApiController]
[Route("api/counter-configs")]
[Authorize(Roles = "Admin")]
public class CounterConfigsController : ControllerBase
{
    private readonly ICounterConfigRepository _repository;

    public CounterConfigsController(ICounterConfigRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<CounterConfigDto>>> GetAll()
    {
        var configs = await _repository.GetAllAsync();
        return Ok(configs.Select(ToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CounterConfigDto>> GetById(Guid id)
    {
        var config = await _repository.GetByIdAsync(id);
        return config is null ? NotFound() : Ok(ToDto(config));
    }

    [HttpPost]
    public async Task<ActionResult<CounterConfigDto>> Create([FromBody] CreateCounterConfigRequest request)
    {
        var existing = await _repository.GetByIdpkAndCounterIdAsync(request.Idpk, request.CounterId);
        if (existing is not null)
        {
            return Conflict(new { message = "Ya existe una configuración para ese parking y contador." });
        }

        var config = new MdcounterConfig
        {
            Idpk = request.Idpk,
            CounterId = request.CounterId,
            CounterName = request.CounterName,
            OccupancyLimit = request.OccupancyLimit,
            CounterType = request.CounterType,
        };

        var created = await _repository.CreateAsync(config);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCounterConfigRequest request)
    {
        var config = new MdcounterConfig
        {
            Id = id,
            CounterName = request.CounterName,
            OccupancyLimit = request.OccupancyLimit,
            CounterType = request.CounterType,
            IsActive = request.IsActive,
        };

        var updated = await _repository.UpdateAsync(config);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    private static CounterConfigDto ToDto(MdcounterConfig config) => new()
    {
        Id = config.Id,
        IsActive = config.IsActive,
        Idpk = config.Idpk,
        CounterId = config.CounterId,
        CounterName = config.CounterName,
        OccupancyLimit = config.OccupancyLimit,
        CounterType = config.CounterType,
    };
}
