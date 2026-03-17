using GoldenGems.Domain.Entities.Security;

namespace GoldenGems.Domain.Entities.People;

/// <summary>
/// Entidad Persona con datos personales del usuario.
/// </summary>
public class Person : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string SecondName { get; set; } = string.Empty;
    public string FirstLastName { get; set; } = string.Empty;
    public string SecondLastName { get; set; } = string.Empty;
    public string DocumentNumber { get; set; } = string.Empty;
    public Guid DocumentTypeId { get; set; }
    public Guid? ContactId { get; set; }
    public Guid UserId { get; set; }
    public DocumentType? DocumentType { get; set; }
    public Contact? Contact { get; set; }
    public User? User { get; set; }
}
