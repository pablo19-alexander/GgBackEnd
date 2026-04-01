using System.ComponentModel.DataAnnotations;

namespace GoldenGems.Application.DTOs.Auth;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email debe tener un formato válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [MinLength(3, ErrorMessage = "El usuario debe tener al menos 3 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "El primer nombre es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El primer nombre debe tener entre 2 y 100 caracteres")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El segundo nombre no puede exceder 100 caracteres")]
    public string? SecondName { get; set; }

    [Required(ErrorMessage = "El primer apellido es requerido")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El primer apellido debe tener entre 2 y 100 caracteres")]
    public string FirstLastName { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "El segundo apellido no puede exceder 100 caracteres")]
    public string? SecondLastName { get; set; }

    public Guid? DocumentTypeId { get; set; }

    [StringLength(50, MinimumLength = 5, ErrorMessage = "El número de documento debe tener entre 5 y 50 caracteres")]
    public string? DocumentNumber { get; set; }

    public List<Guid> RoleIds { get; set; } = new();
}
