namespace GoldenGems.Infrastructure.Authentication;

/// <summary>
/// Configuración JWT para generación y validación de tokens.
/// </summary>
public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public int AccessTokenExpirationMinutes { get; set; }
}
