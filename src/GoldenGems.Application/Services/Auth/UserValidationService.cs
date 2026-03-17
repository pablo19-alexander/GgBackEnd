using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Application.Validators;
using GoldenGems.Domain.Interfaces;

namespace GoldenGems.Application.Services.Auth;

/// <summary>
/// Servicio de validación de datos de usuario.
/// </summary>
public class UserValidationService : IUserValidationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;

    public UserValidationService(
        IUserRepository userRepository,
        IPersonRepository personRepository,
        IRoleRepository roleRepository,
        IDocumentTypeRepository documentTypeRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _roleRepository = roleRepository ?? throw new ArgumentNullException(nameof(roleRepository));
        _documentTypeRepository = documentTypeRepository ?? throw new ArgumentNullException(nameof(documentTypeRepository));
    }

    public async Task<bool> ValidateEmailUniqueAsync(string email, CancellationToken cancellationToken)
    {
        return !await _userRepository.EmailExistsAsync(email, cancellationToken);
    }

    public async Task<bool> ValidateUsernameUniqueAsync(string username, CancellationToken cancellationToken)
    {
        return !await _userRepository.UsernameExistsAsync(username, cancellationToken);
    }

    public bool ValidatePasswordStrength(string password)
    {
        return PasswordValidator.IsValid(password);
    }

    public List<string> GetPasswordValidationErrors(string password)
    {
        return PasswordValidator.GetErrors(password);
    }

    public async Task<bool> ValidateDocumentNumberUniqueAsync(string documentNumber, Guid documentTypeId, CancellationToken cancellationToken)
    {
        return !await _personRepository.DocumentNumberExistsAsync(documentNumber, documentTypeId, cancellationToken);
    }

    public async Task<bool> ValidateRoleExistsAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await _roleRepository.ExistsAsync(roleId, cancellationToken);
    }

    public async Task<bool> ValidateDocumentTypeExistsAsync(Guid documentTypeId, CancellationToken cancellationToken)
    {
        return await _documentTypeRepository.ExistsAsync(documentTypeId, cancellationToken);
    }
}
