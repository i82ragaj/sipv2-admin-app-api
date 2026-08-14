using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.Parkings;

public class CreateParkingRequest
{
    [Required, StringLength(10)]
    public string Id { get; set; } = null!;

    [Required, StringLength(10)]
    public string Type { get; set; } = null!;

    [StringLength(10)]
    public string? Srv { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(100)]
    public string? Company { get; set; }

    [StringLength(10)]
    public string? Dacode { get; set; }

    public DateOnly? DateFromTable { get; set; }

    public DateOnly? DateToTable { get; set; }

    public int? Ndays { get; set; }

    public bool? TruncateTables { get; set; }

    public bool? Sii { get; set; }

    public bool? MultiCounter { get; set; }

    [StringLength(20)]
    public string? ServerIp { get; set; }

    [StringLength(100)]
    public string? Job { get; set; }

    public TimeOnly? LoadDate { get; set; }

    [StringLength(1)]
    public string? Frecuency { get; set; }
}
