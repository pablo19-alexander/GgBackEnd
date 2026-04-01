using GoldenGems.Application.Common;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Auth;

public class ProfileCompletionService : BaseService, IProfileCompletionService
{
    private readonly IPersonRepository _personRepository;

    private const string IncompleteProfileMessage =
        "Debes completar tu perfil antes de continuar. Ve a tu perfil y agrega tu nombre completo, tipo y número de documento.";

    public ProfileCompletionService(IPersonRepository personRepository, ILogger<ProfileCompletionService> logger)
        : base(logger)
    {
        _personRepository = personRepository;
    }

    public async Task<bool> IsProfileCompleteAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var person = await _personRepository.GetByUserIdAsync(userId, cancellationToken);

        if (person == null)
            return false;

        return !string.IsNullOrWhiteSpace(person.FirstName)
            && !string.IsNullOrWhiteSpace(person.FirstLastName)
            && !string.IsNullOrWhiteSpace(person.DocumentNumber)
            && person.DocumentTypeId != Guid.Empty;
    }

    public async Task<ApiResponse<T>> ValidateProfileOrError<T>(Guid userId, CancellationToken cancellationToken = default)
    {
        if (await IsProfileCompleteAsync(userId, cancellationToken))
            return null!;

        _logger.LogWarning("Usuario {UserId} intentó acceder sin perfil completo", userId);
        return ApiResponse<T>.ErrorResponse(IncompleteProfileMessage);
    }
}
