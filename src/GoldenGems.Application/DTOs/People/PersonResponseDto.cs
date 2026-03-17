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
    public Guid? ContactId { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
