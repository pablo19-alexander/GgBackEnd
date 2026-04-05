using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.People;
using GoldenGems.Application.Interfaces.People;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.People;

public class ContactService : BaseService, IContactService
{
    private readonly IContactRepository _contactRepository;

    public ContactService(IContactRepository contactRepository, ILogger<ContactService> logger)
        : base(logger)
    {
        _contactRepository = contactRepository ?? throw new ArgumentNullException(nameof(contactRepository));
    }

    public async Task<ApiResponse<List<ContactResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var contacts = await _contactRepository.GetAllActiveAsync(cancellationToken);
            var dtos = contacts.Select(MapToDto).ToList();
            return ApiResponse<List<ContactResponseDto>>.SuccessResponse(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener contactos");
            return ApiResponse<List<ContactResponseDto>>.ErrorResponse("Error al obtener los contactos");
        }
    }

    public async Task<ApiResponse<ContactResponseDto>> CreateAsync(CreateContactRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<ContactResponseDto>.ErrorResponse("La solicitud es nula");

            if (string.IsNullOrWhiteSpace(request.Mobile))
                return ApiResponse<ContactResponseDto>.ErrorResponse("El número móvil es requerido");

            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                Mobile = request.Mobile.Trim(),
                Email = request.Email?.Trim() ?? string.Empty,
                Address = request.Address?.Trim() ?? string.Empty,
                Neighborhood = request.Neighborhood?.Trim() ?? string.Empty,
                MunicipalityId = request.MunicipalityId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _contactRepository.CreateAsync(contact, cancellationToken);
            var dto = MapToDto(created);

            _logger.LogInformation("Contacto creado exitosamente (ID: {Id})", dto.Id);
            return ApiResponse<ContactResponseDto>.SuccessResponse(dto, "Contacto creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear contacto");
            return ApiResponse<ContactResponseDto>.ErrorResponse("Error al crear el contacto");
        }
    }

    public async Task<ApiResponse<ContactResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdAsync(id, cancellationToken);

            if (contact == null || !contact.IsActive)
                return ApiResponse<ContactResponseDto>.ErrorResponse("Contacto no encontrado");

            return ApiResponse<ContactResponseDto>.SuccessResponse(MapToDto(contact));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener contacto con ID {Id}", id);
            return ApiResponse<ContactResponseDto>.ErrorResponse("Error al obtener el contacto");
        }
    }

    public async Task<ApiResponse<ContactResponseDto>> UpdateAsync(Guid id, CreateContactRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (request == null)
                return ApiResponse<ContactResponseDto>.ErrorResponse("La solicitud es nula");

            var contact = await _contactRepository.GetByIdAsync(id, cancellationToken);

            if (contact == null || !contact.IsActive)
                return ApiResponse<ContactResponseDto>.ErrorResponse("Contacto no encontrado");

            if (!string.IsNullOrWhiteSpace(request.Mobile))
                contact.Mobile = request.Mobile.Trim();

            contact.Email = request.Email?.Trim() ?? contact.Email;
            contact.Address = request.Address?.Trim() ?? contact.Address;
            contact.Neighborhood = request.Neighborhood?.Trim() ?? contact.Neighborhood;
            contact.MunicipalityId = request.MunicipalityId ?? contact.MunicipalityId;

            var updated = await _contactRepository.UpdateAsync(contact, cancellationToken);
            var dto = MapToDto(updated);

            _logger.LogInformation("Contacto actualizado exitosamente (ID: {Id})", dto.Id);
            return ApiResponse<ContactResponseDto>.SuccessResponse(dto, "Contacto actualizado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar contacto con ID {Id}", id);
            return ApiResponse<ContactResponseDto>.ErrorResponse("Error al actualizar el contacto");
        }
    }

    public async Task<ApiResponse<ContactResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var contact = await _contactRepository.GetByIdAsync(id, cancellationToken);

            if (contact == null || !contact.IsActive)
                return ApiResponse<ContactResponseDto>.ErrorResponse("Contacto no encontrado");

            var deleted = await _contactRepository.DeleteAsync(contact, cancellationToken);
            var dto = MapToDto(deleted);

            _logger.LogInformation("Contacto eliminado exitosamente (ID: {Id})", dto.Id);
            return ApiResponse<ContactResponseDto>.SuccessResponse(dto, "Contacto eliminado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar contacto con ID {Id}", id);
            return ApiResponse<ContactResponseDto>.ErrorResponse("Error al eliminar el contacto");
        }
    }

    private static ContactResponseDto MapToDto(Contact contact)
    {
        return new ContactResponseDto
        {
            Id = contact.Id,
            Mobile = contact.Mobile,
            Email = contact.Email,
            Address = contact.Address,
            Neighborhood = contact.Neighborhood,
            MunicipalityId = contact.MunicipalityId,
            IsActive = contact.IsActive,
            CreatedAt = contact.CreatedAt
        };
    }
}
