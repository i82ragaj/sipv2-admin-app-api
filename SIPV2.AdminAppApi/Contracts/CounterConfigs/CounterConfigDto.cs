namespace SIPV2.AdminAppApi.Contracts.CounterConfigs;

public class CounterConfigDto
{
    public Guid Id { get; set; }

    public bool IsActive { get; set; }

    public string Idpk { get; set; } = null!;

    public string CounterId { get; set; } = null!;

    public string? CounterName { get; set; }

    public short? OccupancyLimit { get; set; }

    public string? CounterType { get; set; }
}
