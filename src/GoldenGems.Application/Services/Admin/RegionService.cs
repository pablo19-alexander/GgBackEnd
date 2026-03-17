using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class RegionService : BaseService, IRegionService
{
    private readonly IRegionRepository _regionRepository;

    public RegionService(IRegionRepository regionRepository, ILogger<RegionService> logger)
        : base(logger)
    {
        _regionRepository = regionRepository ?? throw new ArgumentNullException(nameof(regionRepository));
    }

    public async Task<ApiResponse<List<RegionResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var regions = await _regionRepository.GetAllActiveAsync(cancellationToken);
            var dtos = regions.Select(MapToDto).ToList();

            return ApiResponse<List<RegionResponseDto>>.SuccessResponse(
                dtos,
                $"Se encontraron {dtos.Count} regiones"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todas las regiones");
            return ApiResponse<List<RegionResponseDto>>.ErrorResponse("Error al obtener las regiones");
        }
    }

    public async Task<ApiResponse<RegionResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<RegionResponseDto>.ErrorResponse("El ID es inválido");

            var region = await _regionRepository.GetByIdAsync(id, cancellationToken);

            if (region == null || !region.IsActive)
                return ApiResponse<RegionResponseDto>.ErrorResponse("Región no encontrada");

            return ApiResponse<RegionResponseDto>.SuccessResponse(MapToDto(region), "Región encontrada");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener región por ID: {Id}", id);
            return ApiResponse<RegionResponseDto>.ErrorResponse("Error al obtener la región");
        }
    }

    public async Task<ApiResponse<List<RegionResponseDto>>> GetByDepartmentAsync(string department, CancellationToken cancellationToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(department))
                return ApiResponse<List<RegionResponseDto>>.ErrorResponse("El departamento es requerido");

            var regions = await _regionRepository.GetByDepartmentAsync(department, cancellationToken);
            var dtos = regions.Select(MapToDto).ToList();

            return ApiResponse<List<RegionResponseDto>>.SuccessResponse(
                dtos,
                $"Se encontraron {dtos.Count} municipios en el departamento '{department.Trim()}'"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener regiones por departamento: {Department}", department);
            return ApiResponse<List<RegionResponseDto>>.ErrorResponse("Error al obtener las regiones por departamento");
        }
    }

    public async Task<ApiResponse<List<string>>> GetDepartmentsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var departments = await _regionRepository.GetDepartmentsAsync(cancellationToken);

            return ApiResponse<List<string>>.SuccessResponse(
                departments,
                $"Se encontraron {departments.Count} departamentos"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener los departamentos");
            return ApiResponse<List<string>>.ErrorResponse("Error al obtener los departamentos");
        }
    }

    private static RegionResponseDto MapToDto(Region region)
    {
        return new RegionResponseDto
        {
            Id = region.Id,
            Department = region.Department,
            MunicipalityCode = region.MunicipalityCode,
            MunicipalityName = region.MunicipalityName,
            IsActive = region.IsActive
        };
    }
}
