using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Admin;

public class AdminChangePasswordRequestDto
{
    [Required(ErrorMessage = "La nueva contraseña es requerida")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
