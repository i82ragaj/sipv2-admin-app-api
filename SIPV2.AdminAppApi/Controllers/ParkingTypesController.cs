using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts.ParkingTypes;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

// Solo lectura: MDParkingType se mantiene desde base de datos, no desde esta
// API. Lo consumen tanto Configuración (combo de tipo al editar un parking)
// como Consulta (etiqueta de tipo en Estado de integración).
[ApiController]
[Route("api/parking-types")]
[Authorize(Roles = "admin,config,status")]
public class ParkingTypesController : ControllerBase
{
    private readonly IParkingTypeRepository _repository;

    public ParkingTypesController(IParkingTypeRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<List<ParkingTypeDto>>> GetAll()
    {
        var types = await _repository.GetAllAsync();
        return Ok(types.Select(ToDto));
    }

    private static ParkingTypeDto ToDto(MdparkingType type) => new()
    {
        Id = type.Id,
        Name = type.Name,
        Description = type.Description,
        Active = type.Active,
    };
}
