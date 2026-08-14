namespace SIPV2.AdminAppApi.Contracts.Users;

public class UserDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string? LastName { get; set; }

    public string? LastName1 { get; set; }

    public string Login { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public bool Active { get; set; }
}
