using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Admin;

public class CreateActionRequestDto
{
    [Required(ErrorMessage = "El nombre de la acción es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código de la acción es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "El código solo puede contener mayúsculas, números y guiones bajos")]
    public string Code { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El tipo de acción es requerido")]
    public Guid ActionTypeId { get; set; }
}
