using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.Users;

public class UpdateUserRequest
{
    [Required, StringLength(50)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(50)]
    public string? LastName1 { get; set; }

    public string? Email { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    public bool Active { get; set; }
}
