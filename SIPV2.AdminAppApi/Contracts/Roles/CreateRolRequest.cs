using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.Roles;

public class CreateRolRequest
{
    [Required]
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
}
