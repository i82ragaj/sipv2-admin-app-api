using System.ComponentModel.DataAnnotations;

namespace SIPV2.AdminAppApi.Contracts.Users;

// Autoservicio: el propio usuario autenticado cambia su contraseña, verificando la actual.
public class ChangeOwnPasswordRequest
{
    [Required]
    public string CurrentPassword { get; set; } = null!;

    [Required, MinLength(6)]
    public string NewPassword { get; set; } = null!;
}
