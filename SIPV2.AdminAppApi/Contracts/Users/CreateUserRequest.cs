using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.Users;

public class CreateUserRequest
{
    [Required, StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(50)]
    public string? LastName1 { get; set; }

    [Required, StringLength(100)]
    public string Login { get; set; } = null!;

    [Required, MinLength(6)]
    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }
}
