using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.Auth;

public class LoginRequest
{
    [Required]
    public string Login { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
