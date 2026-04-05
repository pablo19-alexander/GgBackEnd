using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Interfaces.Admin;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Admin;

public class DepartmentService : BaseService, IDepartmentService
{
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IDepartmentRepository departmentRepository, ILogger<DepartmentService> logger)
        : base(logger)
    {
        _departmentRepository = departmentRepository ?? throw new ArgumentNullException(nameof(departmentRepository));
    }

    public async Task<ApiResponse<List<DepartmentResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _departmentRepository.GetAllActiveOrderedAsync(cancellationToken);
            var dtos = items.Select(MapToDto).ToList();
            return ApiResponse<List<DepartmentResponseDto>>.SuccessResponse(dtos, $"Se encontraron {dtos.Count} departamentos");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener departamentos");
            return ApiResponse<List<DepartmentResponseDto>>.ErrorResponse("Error al obtener los departamentos");
        }
    }

    public async Task<ApiResponse<DepartmentResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            if (id == Guid.Empty)
                return ApiResponse<DepartmentResponseDto>.ErrorResponse("El ID es inválido");

            var dept = await _departmentRepository.GetByIdAsync(id, cancellationToken);
            if (dept == null || !dept.IsActive)
                return ApiResponse<DepartmentResponseDto>.ErrorResponse("Departamento no encontrado");

            return ApiResponse<DepartmentResponseDto>.SuccessResponse(MapToDto(dept), "Departamento encontrado");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener departamento {Id}", id);
            return ApiResponse<DepartmentResponseDto>.ErrorResponse("Error al obtener el departamento");
        }
    }

    private static DepartmentResponseDto MapToDto(Department d) => new()
    {
        Id = d.Id,
        Code = d.Code,
        Name = d.Name,
        IsActive = d.IsActive
    };
}
