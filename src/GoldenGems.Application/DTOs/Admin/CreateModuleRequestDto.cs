using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Admin;

public class CreateModuleRequestDto
{
    [Required(ErrorMessage = "El código del módulo es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "El código solo puede contener mayúsculas, números y guiones bajos")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del módulo es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    [StringLength(100, ErrorMessage = "El icono no puede exceder 100 caracteres")]
    public string? Icon { get; set; }

    public int DisplayOrder { get; set; }
}
