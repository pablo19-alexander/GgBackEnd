using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.People;

public class UpdatePersonRequestDto
{
    [Required(ErrorMessage = "El primer nombre es requerido")]
    [StringLength(100, ErrorMessage = "El primer nombre no puede exceder 100 caracteres")]
    public string? FirstName { get; set; }

    [StringLength(100, ErrorMessage = "El segundo nombre no puede exceder 100 caracteres")]
    public string? SecondName { get; set; }

    [Required(ErrorMessage = "El primer apellido es requerido")]
    [StringLength(100, ErrorMessage = "El primer apellido no puede exceder 100 caracteres")]
    public string? FirstLastName { get; set; }

    [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder 100 caracteres")]
    public string? SecondLastName { get; set; }

    [Required(ErrorMessage = "El número de documento es requerido")]
    [StringLength(50, ErrorMessage = "El número de documento no puede exceder 50 caracteres")]
    public string? DocumentNumber { get; set; }

    [Required(ErrorMessage = "El tipo de documento es requerido")]
    public Guid? DocumentTypeId { get; set; }

    // ── Datos de contacto (todos obligatorios) ──

    [Required(ErrorMessage = "El número móvil es requerido")]
    [StringLength(20, ErrorMessage = "El número móvil no puede exceder 20 caracteres")]
    public string? Mobile { get; set; }

    [Required(ErrorMessage = "El email es requerido")]
    [StringLength(200, ErrorMessage = "El email no puede exceder 200 caracteres")]
    [EmailAddress(ErrorMessage = "El formato del email no es válido")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "La dirección es requerida")]
    [StringLength(300, ErrorMessage = "La dirección no puede exceder 300 caracteres")]
    public string? Address { get; set; }

    [Required(ErrorMessage = "El barrio es requerido")]
    [StringLength(200, ErrorMessage = "El barrio no puede exceder 200 caracteres")]
    public string? Neighborhood { get; set; }

    [Required(ErrorMessage = "El municipio es requerido")]
    public Guid? MunicipalityId { get; set; }

    // Opcional: solo lo usa el admin para activar/desactivar
    public bool? IsActive { get; set; }
}
