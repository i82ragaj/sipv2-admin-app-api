namespace SIPV2.AdminAppApi.Contracts.Auth;

public class LoginResponse
{
    public string Token { get; set; } = null!;

    public Guid UserId { get; set; }

    public string Name { get; set; } = null!;

    public List<string> Roles { get; set; } = [];
}
