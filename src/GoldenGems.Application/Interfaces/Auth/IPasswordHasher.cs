namespace GoldenGems.Application.Interfaces.Auth;

/// <summary>
/// Abstracción para hashing de contraseñas (inversión de dependencia).
/// </summary>
public interface IPasswordHasherService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string providedPassword, out bool rehashNeeded);
}
