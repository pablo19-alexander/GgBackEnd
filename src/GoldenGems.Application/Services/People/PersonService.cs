using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.People;
using GoldenGems.Application.Interfaces.People;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.People;

public class PersonService : BaseService, IPersonService
{
    private readonly IPersonRepository _personRepository;

    public PersonService(IPersonRepository personRepository, ILogger<PersonService> logger)
        : base(logger)
    {
        _personRepository = personRepository ?? throw new ArgumentNullException(nameof(personRepository));
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
            ContactId = person.ContactId,
            UserId = person.UserId,
            IsActive = person.IsActive,
            CreatedAt = person.CreatedAt
        };
    }
}
