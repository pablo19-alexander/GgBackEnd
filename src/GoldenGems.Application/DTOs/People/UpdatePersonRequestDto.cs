using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.People;

public class UpdatePersonRequestDto
{
    [StringLength(100, ErrorMessage = "El primer nombre no puede exceder 100 caracteres")]
    public string? FirstName { get; set; }

    [StringLength(100, ErrorMessage = "El segundo nombre no puede exceder 100 caracteres")]
    public string? SecondName { get; set; }

    [StringLength(100, ErrorMessage = "El primer apellido no puede exceder 100 caracteres")]
    public string? FirstLastName { get; set; }

    [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder 100 caracteres")]
    public string? SecondLastName { get; set; }

    [StringLength(50, ErrorMessage = "El número de documento no puede exceder 50 caracteres")]
    public string? DocumentNumber { get; set; }

    public Guid? DocumentTypeId { get; set; }
}
