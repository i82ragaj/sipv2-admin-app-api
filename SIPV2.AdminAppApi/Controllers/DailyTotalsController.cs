using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts;
using SIPV2.AdminAppApi.Contracts.DailyTotals;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

// Solo lectura: VDailyTotal es una vista (Ingresos Diarios), no se edita desde la API.
[ApiController]
[Route("api/daily-totals")]
// Grupo "Consulta" del menú (Ingresos Diarios): rol "status", o "admin" (ve todo).
[Authorize(Roles = "admin,status")]
public class DailyTotalsController : ControllerBase
{
    private const int MaxPageSize = 100;

    private readonly IDailyTotalRepository _repository;

    public DailyTotalsController(IDailyTotalRepository repository)
    {
        _repository = repository;
    }

    // Filtro y paginación resueltos en servidor; el listado siempre viene
    // ordenado por fecha descendente (más actuales primero).
    [HttpGet]
    public async Task<ActionResult<PagedResult<DailyTotalDto>>> GetAll(
        [FromQuery] string? parkingId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        pageIndex = Math.Max(pageIndex, 0);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _repository.GetPagedAsync(parkingId, dateFrom, dateTo, pageIndex, pageSize);

        return Ok(new PagedResult<DailyTotalDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = totalCount,
        });
    }

    // Serie sin paginar para el gráfico de evolución (botón "Ver evolución" del
    // listado): últimos 7 días naturales antes del último dato disponible del
    // parking, ascendente por fecha.
    [HttpGet("series")]
    public async Task<ActionResult<List<DailyTotalDto>>> GetSeries([FromQuery] string parkingId)
    {
        if (string.IsNullOrWhiteSpace(parkingId))
        {
            return BadRequest("parkingId es obligatorio.");
        }

        var series = await _repository.GetSeriesAsync(parkingId);
        return Ok(series.Select(ToDto));
    }

    private static DailyTotalDto ToDto(VdailyTotal total) => new()
    {
        Idpk = total.Idpk,
        TotalDate = total.TotalDate,
        DailyTotalAmount = total.DailyTotalAmount,
        DailyTransWithPay = total.DailyTransWithPay,
        DailyTransWithOutPay = total.DailyTransWithOutPay,
    };
}
