namespace GoldenGems.Domain.Entities;

/// <summary>
/// Clase base abstracta para todas las entidades del dominio.
/// </summary>
public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
    }
}
