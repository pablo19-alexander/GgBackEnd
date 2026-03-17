using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.People;

public class CreateContactRequestDto
{
    [Required(ErrorMessage = "El número móvil es requerido")]
    [StringLength(20, ErrorMessage = "El número móvil no puede exceder 20 caracteres")]
    public string Mobile { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "El email no puede exceder 200 caracteres")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string? Email { get; set; }

    [StringLength(300, ErrorMessage = "La dirección no puede exceder 300 caracteres")]
    public string? Address { get; set; }

    [StringLength(200, ErrorMessage = "El barrio no puede exceder 200 caracteres")]
    public string? Neighborhood { get; set; }

    public Guid? RegionId { get; set; }
}
