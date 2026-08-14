namespace SIPV2.AdminAppApi.Contracts.UserRoles;

public class UserRolDto
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public string? UserLogin { get; set; }

    public Guid? RolId { get; set; }

    public string? RolName { get; set; }

    public bool Active { get; set; }
}
