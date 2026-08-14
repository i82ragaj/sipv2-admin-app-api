using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.Users;

// Un Admin fija la contraseña de cualquier usuario, sin necesidad de conocer la actual.
public class ResetPasswordRequest
{
    [Required, MinLength(6)]
    public string NewPassword { get; set; } = null!;
}
