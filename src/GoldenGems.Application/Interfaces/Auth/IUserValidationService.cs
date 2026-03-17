namespace GoldenGems.Application.Interfaces.Auth;

public interface IUserValidationService
{
    Task<bool> ValidateEmailUniqueAsync(string email, CancellationToken cancellationToken);
    Task<bool> ValidateUsernameUniqueAsync(string username, CancellationToken cancellationToken);
    bool ValidatePasswordStrength(string password);
    List<string> GetPasswordValidationErrors(string password);
    Task<bool> ValidateDocumentNumberUniqueAsync(string documentNumber, Guid documentTypeId, CancellationToken cancellationToken);
    Task<bool> ValidateRoleExistsAsync(Guid roleId, CancellationToken cancellationToken);
    Task<bool> ValidateDocumentTypeExistsAsync(Guid documentTypeId, CancellationToken cancellationToken);
}
