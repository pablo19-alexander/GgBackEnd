using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Application.Models;
using GoldenGems.Domain.Entities.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace GoldenGems.Infrastructure.Authentication;

public class JwtTokenService : ITokenService
{
    private readonly JwtSettings _settings;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IOptions<JwtSettings> settings, ILogger<JwtTokenService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public TokenResult GenerateToken(User user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.WriteToken(tokenDescriptor);

        _logger.LogInformation("Issued JWT for user {UserId}", user.Id);
        return new TokenResult(token, expires);
    }
}
