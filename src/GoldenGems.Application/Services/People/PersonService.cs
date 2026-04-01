using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.People;
using GoldenGems.Application.Interfaces.Business;
using GoldenGems.Application.Interfaces.People;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.People;

public class PersonService : BaseService, IPersonService
{
    private readonly IPersonRepository _personRepository;
    private readonly IImageStorageService _imageStorage;

    public PersonService(IPersonRepository personRepository, IImageStorageService imageStorage, ILogger<PersonService> logger)
        : base(logger)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _imageStorage = imageStorage ?? throw new ArgumentNullException(nameof(imageStorage));
    }

    public async Task<ApiResponse<PersonResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _personRepository.GetByIdAsync(id, cancellationToken);

            if (person == null || !person.IsActive)
                return ApiResponse<PersonResponseDto>.ErrorResponse("Persona no encontrada");

            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(person));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener persona con ID {Id}", id);
            return ApiResponse<PersonResponseDto>.ErrorResponse("Error al obtener la persona");
        }
    }

    public async Task<ApiResponse<List<PersonResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var people = await _personRepository.GetAllActiveAsync(cancellationToken);
            var dtos = people.Select(MapToDto).ToList();

            return ApiResponse<List<PersonResponseDto>>.SuccessResponse(
                dtos,
                $"Se encontraron {dtos.Count} personas"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las personas");
            return ApiResponse<List<PersonResponseDto>>.ErrorResponse("Error al obtener las personas");
        }
    }

    public async Task<ApiResponse<PersonResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _personRepository.GetByUserIdAsync(userId, cancellationToken);

            if (person == null || !person.IsActive)
                return ApiResponse<PersonResponseDto>.ErrorResponse("Persona no encontrada para el usuario");

            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(person));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener persona por UserId {UserId}", userId);
            return ApiResponse<PersonResponseDto>.ErrorResponse("Error al obtener la persona");
        }
    }

    public async Task<ApiResponse<PersonResponseDto>> CreateForUserAsync(Guid userId, UpdatePersonRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _personRepository.GetByUserIdAsync(userId, cancellationToken);
            if (existing != null)
                return ApiResponse<PersonResponseDto>.ErrorResponse("Ya existe un perfil para este usuario");

            var person = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                SecondName = request.SecondName?.Trim() ?? string.Empty,
                FirstLastName = request.FirstLastName?.Trim() ?? string.Empty,
                SecondLastName = request.SecondLastName?.Trim() ?? string.Empty,
                DocumentNumber = request.DocumentNumber?.Trim() ?? string.Empty,
                DocumentTypeId = request.DocumentTypeId ?? Guid.Empty,
                UserId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _personRepository.CreateAsync(person, cancellationToken);
            _logger.LogInformation("Perfil creado para usuario {UserId}", userId);
            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(created), "Perfil creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear perfil para usuario {UserId}", userId);
            return ApiResponse<PersonResponseDto>.ErrorResponse("Error al crear el perfil");
        }
    }

    public async Task<ApiResponse<PersonResponseDto>> UpdateAsync(Guid id, UpdatePersonRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<PersonResponseDto>.ErrorResponse("La solicitud es nula");

            var person = await _personRepository.GetByIdAsync(id, cancellationToken);

            if (person == null || !person.IsActive)
                return ApiResponse<PersonResponseDto>.ErrorResponse("Persona no encontrada");

            if (!string.IsNullOrWhiteSpace(request.FirstName))
                person.FirstName = request.FirstName.Trim();

            if (request.SecondName != null)
                person.SecondName = request.SecondName.Trim();

            if (!string.IsNullOrWhiteSpace(request.FirstLastName))
                person.FirstLastName = request.FirstLastName.Trim();

            if (request.SecondLastName != null)
                person.SecondLastName = request.SecondLastName.Trim();

            if (!string.IsNullOrWhiteSpace(request.DocumentNumber))
                person.DocumentNumber = request.DocumentNumber.Trim();

            if (request.DocumentTypeId.HasValue)
                person.DocumentTypeId = request.DocumentTypeId.Value;

            var updated = await _personRepository.UpdateAsync(person, cancellationToken);
            var dto = MapToDto(updated);

            _logger.LogInformation("Persona actualizada exitosamente (ID: {Id})", dto.Id);
            return ApiResponse<PersonResponseDto>.SuccessResponse(dto, "Persona actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar persona con ID {Id}", id);
            return ApiResponse<PersonResponseDto>.ErrorResponse("Error al actualizar la persona");
        }
    }

    public async Task<ApiResponse<PersonResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _personRepository.GetByIdAsync(id, cancellationToken);

            if (person == null || !person.IsActive)
                return ApiResponse<PersonResponseDto>.ErrorResponse("Persona no encontrada");

            var deleted = await _personRepository.DeleteAsync(person, cancellationToken);
            var dto = MapToDto(deleted);

            _logger.LogInformation("Persona eliminada exitosamente (ID: {Id})", dto.Id);
            return ApiResponse<PersonResponseDto>.SuccessResponse(dto, "Persona eliminada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar persona con ID {Id}", id);
            return ApiResponse<PersonResponseDto>.ErrorResponse("Error al eliminar la persona");
        }
    }

    public async Task<ApiResponse<PersonResponseDto>> UploadPhotoAsync(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _personRepository.GetByIdAsync(id, cancellationToken);
            if (person == null || !person.IsActive)
                return ApiResponse<PersonResponseDto>.ErrorResponse("Persona no encontrada");

            // Delete old photo if exists
            if (!string.IsNullOrWhiteSpace(person.PhotoUrl))
                await _imageStorage.DeleteAsync(person.PhotoUrl, cancellationToken);

            var photoUrl = await _imageStorage.UploadAsync(file, "profiles", cancellationToken);
            person.PhotoUrl = photoUrl;

            var updated = await _personRepository.UpdateAsync(person, cancellationToken);
            _logger.LogInformation("Foto de perfil actualizada (Person ID: {Id})", id);
            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(updated), "Foto de perfil actualizada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir foto de perfil (Person ID: {Id})", id);
            return ApiResponse<PersonResponseDto>.ErrorResponse("Error al subir la foto de perfil");
        }
    }

    public async Task<ApiResponse<PersonResponseDto>> DeletePhotoAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _personRepository.GetByIdAsync(id, cancellationToken);
            if (person == null || !person.IsActive)
                return ApiResponse<PersonResponseDto>.ErrorResponse("Persona no encontrada");

            if (!string.IsNullOrWhiteSpace(person.PhotoUrl))
            {
                await _imageStorage.DeleteAsync(person.PhotoUrl, cancellationToken);
                person.PhotoUrl = null;
                await _personRepository.UpdateAsync(person, cancellationToken);
            }

            _logger.LogInformation("Foto de perfil eliminada (Person ID: {Id})", id);
            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(person), "Foto de perfil eliminada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar foto de perfil (Person ID: {Id})", id);
            return ApiResponse<PersonResponseDto>.ErrorResponse("Error al eliminar la foto de perfil");
        }
    }

    private static PersonResponseDto MapToDto(Person person)
    {
        return new PersonResponseDto
        {
            Id = person.Id,
            FirstName = person.FirstName,
            SecondName = person.SecondName,
            FirstLastName = person.FirstLastName,
            SecondLastName = person.SecondLastName,
            DocumentNumber = person.DocumentNumber,
            DocumentTypeId = person.DocumentTypeId,
            PhotoUrl = person.PhotoUrl,
            ContactId = person.ContactId,
            UserId = person.UserId,
            IsActive = person.IsActive,
            CreatedAt = person.CreatedAt
        };
    }
}
