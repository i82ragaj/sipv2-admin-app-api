using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.CounterConfigs;

public class UpdateCounterConfigRequest
{
    public bool IsActive { get; set; }

    [StringLength(50)]
    public string? CounterName { get; set; }

    public short? OccupancyLimit { get; set; }

    [StringLength(10)]
    public string? CounterType { get; set; }
}
