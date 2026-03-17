using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Admin;

public class CreateRoleRequestDto
{
    [Required(ErrorMessage = "El nombre del rol es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }
}
