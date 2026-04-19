using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Admin;

public class CreateActionTypeRequestDto
{
    [Required(ErrorMessage = "El código del tipo de acción es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "El código solo puede contener mayúsculas, números y guiones bajos")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida")]
    [StringLength(500, MinimumLength = 3, ErrorMessage = "La descripción debe tener entre 3 y 500 caracteres")]
    public string Description { get; set; } = string.Empty;
}
