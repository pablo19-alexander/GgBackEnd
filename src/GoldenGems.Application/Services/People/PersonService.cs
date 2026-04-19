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
    private readonly IContactRepository _contactRepository;
    private readonly IImageStorageService _imageStorage;

    public PersonService(
        IPersonRepository personRepository,
        IContactRepository contactRepository,
        IImageStorageService imageStorage,
        ILogger<PersonService> logger)
        : base(logger)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
        _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
        _imageStorage = imageStorage ?? throw new ArgumentNullException(nameof(imageStorage));
    }

    public async Task<ApiResponse<PersonResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var person = await _personRepository.GetByIdAsync(id, cancellationToken);

            if (person == null || !person.IsActive)
                return ApiResponse<PersonResponseDto>.ErrorResponse("Persona no encontrada");

            var contact = person.ContactId.HasValue
                ? await _contactRepository.GetByIdAsync(person.ContactId.Value, cancellationToken)
                : null;

            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(person, contact));
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
            var dtos = new List<PersonResponseDto>();
            foreach (var person in people)
            {
                var contact = person.ContactId.HasValue
                    ? await _contactRepository.GetByIdAsync(person.ContactId.Value, cancellationToken)
                    : null;
                dtos.Add(MapToDto(person, contact));
            }

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

            var contact = person.ContactId.HasValue
                ? await _contactRepository.GetByIdAsync(person.ContactId.Value, cancellationToken)
                : null;

            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(person, contact));
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

            // Crear Contact asociado
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Mobile = request.Mobile?.Trim() ?? string.Empty,
                Email = request.Email?.Trim() ?? string.Empty,
                Address = request.Address?.Trim() ?? string.Empty,
                Neighborhood = request.Neighborhood?.Trim() ?? string.Empty,
                MunicipalityId = request.MunicipalityId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var createdContact = await _contactRepository.CreateAsync(contact, cancellationToken);

            var person = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName?.Trim() ?? string.Empty,
                SecondName = request.SecondName?.Trim() ?? string.Empty,
                FirstLastName = request.FirstLastName?.Trim() ?? string.Empty,
                SecondLastName = request.SecondLastName?.Trim() ?? string.Empty,
                DocumentNumber = request.DocumentNumber?.Trim() ?? string.Empty,
                DocumentTypeId = request.DocumentTypeId ?? Guid.Empty,
                ContactId = createdContact.Id,
                UserId = userId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _personRepository.CreateAsync(person, cancellationToken);
            _logger.LogInformation("Perfil creado para usuario {UserId} (Contact: {ContactId})", userId, createdContact.Id);
            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(created, createdContact), "Perfil creado exitosamente");
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

            if (person == null)
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

            if (request.IsActive.HasValue)
                person.IsActive = request.IsActive.Value;

            // Crear o actualizar Contact asociado
            Contact? contact = null;
            if (person.ContactId.HasValue)
            {
                contact = await _contactRepository.GetByIdAsync(person.ContactId.Value, cancellationToken);
            }

            if (contact == null)
            {
                contact = new Contact
                {
                    Id = Guid.NewGuid(),
                    Mobile = request.Mobile?.Trim() ?? string.Empty,
                    Email = request.Email?.Trim() ?? string.Empty,
                    Address = request.Address?.Trim() ?? string.Empty,
                    Neighborhood = request.Neighborhood?.Trim() ?? string.Empty,
                    MunicipalityId = request.MunicipalityId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                contact = await _contactRepository.CreateAsync(contact, cancellationToken);
                person.ContactId = contact.Id;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(request.Mobile))
                    contact.Mobile = request.Mobile.Trim();
                if (request.Email != null)
                    contact.Email = request.Email.Trim();
                if (request.Address != null)
                    contact.Address = request.Address.Trim();
                if (request.Neighborhood != null)
                    contact.Neighborhood = request.Neighborhood.Trim();
                if (request.MunicipalityId.HasValue)
                    contact.MunicipalityId = request.MunicipalityId.Value;
                contact = await _contactRepository.UpdateAsync(contact, cancellationToken);
            }

            var updated = await _personRepository.UpdateAsync(person, cancellationToken);
            var dto = MapToDto(updated, contact);

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
            var dto = MapToDto(deleted, null);

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
            var contact = updated.ContactId.HasValue
                ? await _contactRepository.GetByIdAsync(updated.ContactId.Value, cancellationToken)
                : null;
            _logger.LogInformation("Foto de perfil actualizada (Person ID: {Id})", id);
            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(updated, contact), "Foto de perfil actualizada exitosamente");
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

            var contact = person.ContactId.HasValue
                ? await _contactRepository.GetByIdAsync(person.ContactId.Value, cancellationToken)
                : null;

            _logger.LogInformation("Foto de perfil eliminada (Person ID: {Id})", id);
            return ApiResponse<PersonResponseDto>.SuccessResponse(MapToDto(person, contact), "Foto de perfil eliminada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar foto de perfil (Person ID: {Id})", id);
            return ApiResponse<PersonResponseDto>.ErrorResponse("Error al eliminar la foto de perfil");
        }
    }

    private static PersonResponseDto MapToDto(Person person, Contact? contact)
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
            CreatedAt = person.CreatedAt,
            Mobile = contact?.Mobile,
            Email = contact?.Email,
            Address = contact?.Address,
            Neighborhood = contact?.Neighborhood,
            MunicipalityId = contact?.MunicipalityId
        };
    }
}
