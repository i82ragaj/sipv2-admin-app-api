namespace SIPV2.AdminAppApi.Contracts.Roles;

public class UpdateRolRequest
{
    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool Active { get; set; }
}
