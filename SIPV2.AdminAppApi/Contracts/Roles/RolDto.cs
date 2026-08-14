namespace SIPV2.AdminAppApi.Contracts.Roles;

public class RolDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool Active { get; set; }
}
