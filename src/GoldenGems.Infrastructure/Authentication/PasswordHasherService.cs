using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Domain.Entities.Security;
using Microsoft.AspNetCore.Identity;

namespace GoldenGems.Infrastructure.Authentication;

/// <summary>
/// Implementación del servicio de hashing usando ASP.NET Identity PasswordHasher.
/// </summary>
public class PasswordHasherService : IPasswordHasherService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    private static readonly User _dummyUser = new();

    public PasswordHasherService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(_dummyUser, password);
    }

    public bool VerifyPassword(string hashedPassword, string providedPassword, out bool rehashNeeded)
    {
        var result = _passwordHasher.VerifyHashedPassword(_dummyUser, hashedPassword, providedPassword);
        rehashNeeded = result == PasswordVerificationResult.SuccessRehashNeeded;
        return result != PasswordVerificationResult.Failed;
    }
}
