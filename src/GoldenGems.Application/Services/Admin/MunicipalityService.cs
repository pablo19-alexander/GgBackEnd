using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class MunicipalityService : BaseService, IMunicipalityService
{
    private readonly IMunicipalityRepository _municipalityRepository;

    public MunicipalityService(IMunicipalityRepository municipalityRepository, ILogger<MunicipalityService> logger)
        : base(logger)
    {
        _municipalityRepository = municipalityRepository ?? throw new ArgumentNullException(nameof(municipalityRepository));
    }

    public async Task<ApiResponse<List<MunicipalityResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _municipalityRepository.GetAllActiveWithDepartmentAsync(cancellationToken);
            var dtos = items.Select(MapToDto).ToList();
            return ApiResponse<List<MunicipalityResponseDto>>.SuccessResponse(dtos, $"Se encontraron {dtos.Count} municipios");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener municipios");
            return ApiResponse<List<MunicipalityResponseDto>>.ErrorResponse("Error al obtener los municipios");
        }
    }

    public async Task<ApiResponse<MunicipalityResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<MunicipalityResponseDto>.ErrorResponse("El ID es inválido");

            var muni = await _municipalityRepository.GetByIdAsync(id, cancellationToken);
            if (muni == null || !muni.IsActive)
                return ApiResponse<MunicipalityResponseDto>.ErrorResponse("Municipio no encontrado");

            // Re-fetch with department eager loaded if needed
            if (muni.Department == null)
            {
                var all = await _municipalityRepository.GetAllActiveWithDepartmentAsync(cancellationToken);
                muni = all.FirstOrDefault(m => m.Id == id) ?? muni;
            }

            return ApiResponse<MunicipalityResponseDto>.SuccessResponse(MapToDto(muni), "Municipio encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener municipio {Id}", id);
            return ApiResponse<MunicipalityResponseDto>.ErrorResponse("Error al obtener el municipio");
        }
    }

    public async Task<ApiResponse<List<MunicipalityResponseDto>>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken)
    {
        try
        {
            if (departmentId == Guid.Empty)
                return ApiResponse<List<MunicipalityResponseDto>>.ErrorResponse("El departamento es requerido");

            var items = await _municipalityRepository.GetByDepartmentIdAsync(departmentId, cancellationToken);
            var dtos = items.Select(MapToDto).ToList();
            return ApiResponse<List<MunicipalityResponseDto>>.SuccessResponse(dtos, $"Se encontraron {dtos.Count} municipios");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener municipios por departamento {DepartmentId}", departmentId);
            return ApiResponse<List<MunicipalityResponseDto>>.ErrorResponse("Error al obtener los municipios por departamento");
        }
    }

    private static MunicipalityResponseDto MapToDto(Municipality m) => new()
    {
        Id = m.Id,
        Code = m.Code,
        Name = m.Name,
        DepartmentId = m.DepartmentId,
        DepartmentName = m.Department?.Name ?? string.Empty,
        Latitude = m.Latitude,
        Longitude = m.Longitude,
        IsActive = m.IsActive
    };
}
