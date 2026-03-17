using System.Text.RegularExpressions;

namespace GoldenGems.Application.Validators;

/// <summary>
/// Validador de contraseñas fuertes.
/// </summary>
public static class PasswordValidator
{
    private const int MinimumLength = 8;
    private const string SpecialCharacters = "!@#$%^&*";

    public static (bool isValid, List<string> errors) Validate(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("La contraseña no puede estar vacía");
            return (false, errors);
        }

        if (password.Length < MinimumLength)
            errors.Add($"La contraseña debe tener al menos {MinimumLength} caracteres");

        if (!Regex.IsMatch(password, @"[A-Z]"))
            errors.Add("La contraseña debe contener al menos una mayúscula (A-Z)");

        if (!Regex.IsMatch(password, @"[a-z]"))
            errors.Add("La contraseña debe contener al menos una minúscula (a-z)");

        if (!Regex.IsMatch(password, @"[0-9]"))
            errors.Add("La contraseña debe contener al menos un número (0-9)");

        if (!Regex.IsMatch(password, $@"[{Regex.Escape(SpecialCharacters)}]"))
            errors.Add($"La contraseña debe contener al menos un carácter especial ({SpecialCharacters})");

        return (errors.Count == 0, errors);
    }

    public static bool IsValid(string password)
    {
        var (isValid, _) = Validate(password);
        return isValid;
    }

    public static List<string> GetErrors(string password)
    {
        var (_, errors) = Validate(password);
        return errors;
    }
}
