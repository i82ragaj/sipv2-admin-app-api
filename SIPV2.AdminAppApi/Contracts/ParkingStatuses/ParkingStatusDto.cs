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

    // Del join con MDParking (mismo Id).
    public bool? ParkingActive { get; set; }

    public string? ParkingName { get; set; }

    public string? ParkingType { get; set; }

    public string? ParkingDacode { get; set; }

    public string? ParkingServerIp { get; set; }

    public string? ParkingJob { get; set; }

    public TimeOnly? ParkingLoadDate { get; set; }
}
