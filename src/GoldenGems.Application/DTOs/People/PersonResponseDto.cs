namespace GoldenGems.Application.DTOs.People;

public class PersonResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? SecondName { get; set; }
    public string FirstLastName { get; set; } = string.Empty;
    public string? SecondLastName { get; set; }
    public string? DocumentNumber { get; set; }
    public Guid DocumentTypeId { get; set; }
    public string? PhotoUrl { get; set; }
    public Guid? ContactId { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    // ── Datos de contacto (desde la tabla Contact) ──
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? Neighborhood { get; set; }
    public Guid? MunicipalityId { get; set; }
}
