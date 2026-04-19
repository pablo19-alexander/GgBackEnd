using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Business;

public class AdminCreateCompanyRequestDto
{
    [Required(ErrorMessage = "El OwnerId es requerido")]
    public Guid OwnerId { get; set; }

    [Required(ErrorMessage = "El nombre es requerido")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 200 caracteres")]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public string Logo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El NIT es requerido")]
    [StringLength(50, MinimumLength = 5, ErrorMessage = "El NIT debe tener entre 5 y 50 caracteres")]
    public string NIT { get; set; } = string.Empty;

    [StringLength(20)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "El email debe tener un formato válido")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string WhatsAppNumber { get; set; } = string.Empty;

    public bool IsDefault { get; set; } = false;
}
