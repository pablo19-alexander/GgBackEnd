using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Business;

public class CreateProductTypeRequestDto
{
    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "El código es requerido")]
    [StringLength(20, MinimumLength = 1)]
    public string Code { get; set; } = string.Empty;

    [StringLength(300)]
    public string Description { get; set; } = string.Empty;

    [StringLength(50)]
    public string Icon { get; set; } = string.Empty;
}
