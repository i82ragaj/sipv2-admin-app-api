using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.UserRoles;

public class CreateUserRolRequest
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid RolId { get; set; }
}
