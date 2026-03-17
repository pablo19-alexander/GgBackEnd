namespace GoldenGems.Application.DTOs.Auth;

public class AuthResponseDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
