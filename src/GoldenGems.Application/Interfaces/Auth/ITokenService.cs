using GoldenGems.Domain.Entities.Security;
using GoldenGems.Application.Models;

namespace GoldenGems.Application.Interfaces.Auth;

public interface ITokenService
{
    TokenResult GenerateToken(User user, IEnumerable<string> roles);
}
