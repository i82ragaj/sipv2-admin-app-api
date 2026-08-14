using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.CounterConfigs;

public class CreateCounterConfigRequest
{
    [Required, StringLength(10)]
    public string Idpk { get; set; } = null!;

    [Required, StringLength(40)]
    public string CounterId { get; set; } = null!;

    [StringLength(50)]
    public string? CounterName { get; set; }

    public short? OccupancyLimit { get; set; }

    [StringLength(10)]
    public string? CounterType { get; set; }
}
