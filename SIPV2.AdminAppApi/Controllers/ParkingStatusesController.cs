using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.ParkingStatuses;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

// Solo lectura: MDParkingStatus lo alimenta el proceso de importación, no se edita desde la API.
[ApiController]
[Route("api/parking-statuses")]
[Authorize(Roles = "admin")]
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
    };
}
