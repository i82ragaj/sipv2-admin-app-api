using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.Parkings;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

[ApiController]
[Route("api/parkings")]
[Authorize(Roles = "admin")]
public class ParkingsController : ControllerBase
{
    private readonly IParkingRepository _repository;

    public ParkingsController(IParkingRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ParkingDto>>> GetAll()
    {
        var parkings = await _repository.GetAllAsync();
        return Ok(parkings.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ParkingDto>> GetById(string id)
    {
        var parking = await _repository.GetByIdAsync(id);
        return parking is null ? NotFound() : Ok(ToDto(parking));
    }

    [HttpPost]
    public async Task<ActionResult<ParkingDto>> Create([FromBody] CreateParkingRequest request)
    {
        var existing = await _repository.GetByIdAsync(request.Id);
        if (existing is not null)
        {
            return Conflict(new { message = "Ya existe un parking con ese Id." });
        }

        var parking = new Mdparking
        {
            Id = request.Id,
            Type = request.Type,
            Srv = request.Srv,
            Name = request.Name,
            Company = request.Company,
            Dacode = request.Dacode,
            DateFromTable = request.DateFromTable,
            DateToTable = request.DateToTable,
            Ndays = request.Ndays,
            TruncateTables = request.TruncateTables,
            Sii = request.Sii,
            MultiCounter = request.MultiCounter,
            ServerIp = request.ServerIp,
            Job = request.Job,
            LoadDate = request.LoadDate,
            Frecuency = request.Frecuency,
        };

        var created = await _repository.CreateAsync(parking);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(created));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateParkingRequest request)
    {
        var parking = new Mdparking
        {
            Id = id,
            Active = request.Active,
            Type = request.Type,
            Srv = request.Srv,
            Name = request.Name,
            Company = request.Company,
            Dacode = request.Dacode,
            DateFromTable = request.DateFromTable,
            DateToTable = request.DateToTable,
            Ndays = request.Ndays,
            TruncateTables = request.TruncateTables,
            Sii = request.Sii,
            MultiCounter = request.MultiCounter,
            ServerIp = request.ServerIp,
            Job = request.Job,
            LoadDate = request.LoadDate,
            Frecuency = request.Frecuency,
        };

        var updated = await _repository.UpdateAsync(parking);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _repository.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    private static ParkingDto ToDto(Mdparking parking) => new()
    {
        Id = parking.Id,
        Active = parking.Active,
        Type = parking.Type,
        Srv = parking.Srv,
        Name = parking.Name,
        Company = parking.Company,
        Dacode = parking.Dacode,
        DateFromTable = parking.DateFromTable,
        DateToTable = parking.DateToTable,
        Ndays = parking.Ndays,
        TruncateTables = parking.TruncateTables,
        Sii = parking.Sii,
        MultiCounter = parking.MultiCounter,
        ServerIp = parking.ServerIp,
        Job = parking.Job,
        LoadDate = parking.LoadDate,
        Frecuency = parking.Frecuency,
    };
}
