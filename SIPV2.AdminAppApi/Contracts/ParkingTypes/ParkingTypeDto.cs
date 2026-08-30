namespace SIPV2.AdminAppApi.Contracts.ParkingTypes;

public class ParkingTypeDto
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool Active { get; set; }
}
