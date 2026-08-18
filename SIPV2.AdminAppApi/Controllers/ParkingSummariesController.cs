using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SIPV2.AdminAppApi.Contracts;
using SIPV2.AdminAppApi.Contracts.ParkingSummaries;
using SIPV2.AdminAppApi.Services;
using SIPV2.DataModels;

namespace SIPV2.AdminAppApi.Controllers;

// Solo lectura: VParkingSummary es una vista (Informe ERP), no se edita desde la API.
[ApiController]
[Route("api/parking-summaries")]
// Grupo "Consulta" del menú (Informe ERP): rol "status", o "admin" (ve todo).
[Authorize(Roles = "admin,status")]
public class ParkingSummariesController : ControllerBase
{
    private const int MaxPageSize = 100;

    private readonly IParkingSummaryRepository _repository;

    public ParkingSummariesController(IParkingSummaryRepository repository)
    {
        _repository = repository;
    }

    // Filtro y paginación resueltos en servidor; el listado siempre viene
    // ordenado por fecha descendente (más actuales primero).
    [HttpGet]
    public async Task<ActionResult<PagedResult<ParkingSummaryDto>>> GetAll(
        [FromQuery] string? parkingId,
        [FromQuery] DateOnly? dateFrom,
        [FromQuery] DateOnly? dateTo,
        [FromQuery] int pageIndex = 0,
        [FromQuery] int pageSize = 10)
    {
        pageIndex = Math.Max(pageIndex, 0);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);

        var (items, totalCount) = await _repository.GetPagedAsync(parkingId, dateFrom, dateTo, pageIndex, pageSize);

        return Ok(new PagedResult<ParkingSummaryDto>
        {
            Items = items.Select(ToDto).ToList(),
            TotalCount = totalCount,
        });
    }

    private static ParkingSummaryDto ToDto(VparkingSummary summary) => new()
    {
        Id = summary.Id,
        Idpk = summary.Idpk,
        Date = summary.Date,
        Operations = summary.Operations,
        Total = summary.Total,
        InvoiceNoMin = summary.InvoiceNoMin,
        InvoiceNoMax = summary.InvoiceNoMax,
        ParkingTransTotal = summary.ParkingTransTotal,
        ParkingTransNum = summary.ParkingTransNum,
        SumMinutes = summary.SumMinutes,
        InvoicesTotal = summary.InvoicesTotal,
        InvoicesNum = summary.InvoicesNum,
        Acatotal = summary.Acatotal,
        Acanum = summary.Acanum,
        Ceitotal = summary.Ceitotal,
        Ceinum = summary.Ceinum,
        Certotal = summary.Certotal,
        Cernum = summary.Cernum,
        CashTotal = summary.CashTotal,
        CashNum = summary.CashNum,
        RestTotal = summary.RestTotal,
        RestNum = summary.RestNum,
        DiscountTotal = summary.DiscountTotal,
        DiscountNum = summary.DiscountNum,
    };
}
