namespace SIPV2.AdminAppApi.Contracts.ParkingStatuses;

public class ParkingStatusDto
{
    public string Id { get; set; } = null!;

    public bool Active { get; set; }

    public DateTime? LastImported { get; set; }

    public string? LastImportedStatus { get; set; }

    public DateTime? LastImportedOk { get; set; }

    public DateTime? LastCountTotals { get; set; }

    public DateTime? LastCountTotalsStatus { get; set; }

    public DateTime? LastImportedDuration { get; set; }
}
