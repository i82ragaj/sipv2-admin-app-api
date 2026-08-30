namespace SIPV2.AdminAppApi.Contracts.Occupancy;

// Refleja VOccupationActual: ocupación en tiempo real por contador/parking.
public class CurrentOccupancyDto
{
    public string ParkingId { get; set; } = null!;

    public string? ParkingName { get; set; }

    public string CounterId { get; set; } = null!;

    public string CounterCode { get; set; } = null!;

    public string? CounterName { get; set; }

    public short? Capacity { get; set; }

    public short CurrentLevel { get; set; }

    public decimal? Percentage { get; set; }

    public DateTime? Updated { get; set; }
}
