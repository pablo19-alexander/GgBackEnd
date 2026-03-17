using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Admin;

public class CreateDocumentTypeRequestDto
{
    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "El código debe tener entre 1 y 20 caracteres")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
    public string Name { get; set; } = string.Empty;
}
