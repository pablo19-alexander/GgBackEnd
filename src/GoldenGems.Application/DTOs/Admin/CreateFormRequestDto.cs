using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Admin;

public class CreateFormRequestDto
{
    [Required(ErrorMessage = "El código del formulario es requerido")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El código debe tener entre 3 y 50 caracteres")]
    [RegularExpression(@"^[A-Z0-9_]+$", ErrorMessage = "El código solo puede contener mayúsculas, números y guiones bajos")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "La referencia del formulario es requerida")]
    [StringLength(200, MinimumLength = 3, ErrorMessage = "La referencia debe tener entre 3 y 200 caracteres")]
    public string FormReference { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del formulario es requerido")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
    public string? Description { get; set; }

    [StringLength(200, ErrorMessage = "La ruta no puede exceder 200 caracteres")]
    public string? Route { get; set; }

    [Required(ErrorMessage = "El módulo es requerido")]
    public Guid ModuleId { get; set; }

    public int DisplayOrder { get; set; }
}
