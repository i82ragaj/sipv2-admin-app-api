using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Occupancy;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

// Solo lectura: VOccupationActual es una vista (Ocupación Actual), no se edita desde la API.
[ApiController]
[Route("api/occupancy")]
// Grupo "Consulta" del menú (Ocupación Actual): rol "status", o "admin" (ve todo).
[Authorize(Roles = "admin,status")]
public class OccupancyController : ControllerBase
{
    private readonly IOccupancyRepository _repository;

    public OccupancyController(IOccupancyRepository repository)
    {
        _repository = repository;
    }

    // Foto del estado actual (sin paginar): un contador puede filtrarse por
    // parking y/o por nombre (coincidencia parcial, ambos opcionales).
    [HttpGet("current")]
    public async Task<ActionResult<List<CurrentOccupancyDto>>> GetCurrent(
        [FromQuery] string? parkingId,
        [FromQuery] string? counterName)
    {
        var occupancy = await _repository.GetCurrentAsync(parkingId, counterName);
        return Ok(occupancy.Select(ToDto));
    }

    private static CurrentOccupancyDto ToDto(VoccupationActual o) => new()
    {
        ParkingId = o.ParkingId,
        ParkingName = o.ParkingName,
        CounterId = o.CounterId,
        CounterCode = o.CounterCode,
        CounterName = o.CounterName,
        Capacity = o.Capacity,
        CurrentLevel = o.CurrentLevel,
        Percentage = o.Perc,
        Updated = o.Updated,
    };
}
