namespace SIPV2.AdminAppApi.Contracts.Parkings;

public class ParkingDto
{
    public string Id { get; set; } = null!;

    public bool Active { get; set; }

    public string Type { get; set; } = null!;

    public string? Srv { get; set; }

    public string? Name { get; set; }

    public string? Company { get; set; }

    public string? Dacode { get; set; }

    public DateOnly? DateFromTable { get; set; }

    public DateOnly? DateToTable { get; set; }

    public int? Ndays { get; set; }

    public bool? TruncateTables { get; set; }

    public bool? Sii { get; set; }

    public bool? MultiCounter { get; set; }

    public string? ServerIp { get; set; }

    public string? Job { get; set; }

    public TimeOnly? LoadDate { get; set; }

    public string? Frecuency { get; set; }
}
