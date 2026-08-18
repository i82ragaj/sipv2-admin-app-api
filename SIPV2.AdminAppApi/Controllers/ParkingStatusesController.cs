using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.ParkingStatuses;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

// MDParkingStatus lo alimenta el proceso de importación; la única escritura
// permitida desde la API es solicitar una importación diaria (RequestDailyImport).
[ApiController]
[Route("api/parking-statuses")]
// Grupo "Estado" del menú (Estado de parkings): rol "status", o "admin" (ve todo).
[Authorize(Roles = "admin,status")]
public class ParkingStatusesController : ControllerBase
{
    private readonly IParkingStatusRepository _repository;

    public ParkingStatusesController(IParkingStatusRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ParkingStatusDto>>> GetAll()
    {
        var statuses = await _repository.GetAllAsync();
        return Ok(statuses.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ParkingStatusDto>> GetById(string id)
    {
        var status = await _repository.GetByIdAsync(id);
        return status is null ? NotFound() : Ok(ToDto(status));
    }

    // Única escritura permitida: marca LastImportedStatus = "PENDIENTE" para
    // que el proceso de importación lo recoja. Solo si el último estado era
    // "OK" o "ERROR" (si ya está PENDIENTE, no se vuelve a pedir).
    [HttpPut("{id}/daily-import")]
    public async Task<IActionResult> RequestDailyImport(string id)
    {
        var result = await _repository.RequestDailyImportAsync(id);
        return result switch
        {
            RequestDailyImportResult.Success => NoContent(),
            RequestDailyImportResult.NotAllowed => Conflict(
                new { message = "Solo se puede solicitar la importación diaria cuando el último estado es OK o ERROR." }),
            _ => NotFound(),
        };
    }

    private static ParkingStatusDto ToDto(MdparkingStatus status) => new()
    {
        Id = status.Id,
        Active = status.Active,
        LastImported = status.LastImported,
        LastImportedStatus = status.LastImportedStatus,
        LastImportedOk = status.LastImportedOk,
        LastCountTotals = status.LastCountTotals,
        LastCountTotalsStatus = status.LastCountTotalsStatus,
        LastImportedDuration = status.LastImportedDuration,
        ParkingActive = status.Parking?.Active,
        ParkingName = status.Parking?.Name,
        ParkingType = status.Parking?.Type,
        ParkingDacode = status.Parking?.Dacode,
        ParkingServerIp = status.Parking?.ServerIp,
        ParkingJob = status.Parking?.Job,
        ParkingLoadDate = status.Parking?.LoadDate,
    };
}
