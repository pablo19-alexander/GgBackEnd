namespace GoldenGems.Application.DTOs.Admin;

/// <summary>
/// Usuario disponible para vincular una Persona nueva desde la vista administrativa.
/// Se expone únicamente la información mínima necesaria para el selector.
/// </summary>
public class AvailableUserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
