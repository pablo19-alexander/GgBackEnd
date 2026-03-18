using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Payment;
using GoldenGems.Application.Interfaces.Payment;
using GoldenGems.Domain.Entities.Payment;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Payment;

public class CommissionService : BaseService, ICommissionService
{
    private readonly ICommissionRepository _commissionRepository;
    private readonly ICompanyRepository _companyRepository;

    public CommissionService(ICommissionRepository commissionRepository, ICompanyRepository companyRepository, ILogger<CommissionService> logger) : base(logger)
    {
        _commissionRepository = commissionRepository;
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<CommissionResponseDto>> CreateAsync(CommissionRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            if (!await _companyRepository.ExistsAsync(request.CompanyId, cancellationToken))
                return ApiResponse<CommissionResponseDto>.ErrorResponse("La empresa no existe.");

            var existing = await _commissionRepository.GetByCompanyIdAsync(request.CompanyId, cancellationToken);
            if (existing != null)
                return ApiResponse<CommissionResponseDto>.ErrorResponse("Ya existe una comisión configurada para esta empresa.");

            var commission = new Commission
            {
                CompanyId = request.CompanyId,
                Percentage = request.Percentage,
                MinAmount = request.MinAmount,
                MaxAmount = request.MaxAmount
            };

            var created = await _commissionRepository.CreateAsync(commission, cancellationToken);
            var detail = await _commissionRepository.GetByCompanyIdAsync(request.CompanyId, cancellationToken);
            return ApiResponse<CommissionResponseDto>.SuccessResponse(MapToDto(detail!), "Comisión creada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear comisión");
            return ApiResponse<CommissionResponseDto>.ErrorResponse("Error al crear la comisión.");
        }
    }

    public async Task<ApiResponse<List<CommissionResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _commissionRepository.GetAllActiveAsync(cancellationToken);
            return ApiResponse<List<CommissionResponseDto>>.SuccessResponse(items.Select(MapToDto).ToList(), $"Se encontraron {items.Count} comisiones");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener comisiones");
            return ApiResponse<List<CommissionResponseDto>>.ErrorResponse("Error al obtener las comisiones.");
        }
    }

    public async Task<ApiResponse<CommissionResponseDto>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            var commission = await _commissionRepository.GetByCompanyIdAsync(companyId, cancellationToken);
            if (commission == null)
                return ApiResponse<CommissionResponseDto>.ErrorResponse("No hay comisión configurada para esta empresa.");

            return ApiResponse<CommissionResponseDto>.SuccessResponse(MapToDto(commission), "Comisión encontrada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener comisión: {CompanyId}", companyId);
            return ApiResponse<CommissionResponseDto>.ErrorResponse("Error al obtener la comisión.");
        }
    }

    public async Task<ApiResponse<CommissionResponseDto>> UpdateAsync(Guid id, CommissionRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var commission = await _commissionRepository.GetByIdAsync(id, cancellationToken);
            if (commission == null || !commission.IsActive)
                return ApiResponse<CommissionResponseDto>.ErrorResponse("Comisión no encontrada.");

            commission.Percentage = request.Percentage;
            commission.MinAmount = request.MinAmount;
            commission.MaxAmount = request.MaxAmount;

            await _commissionRepository.UpdateAsync(commission, cancellationToken);
            var detail = await _commissionRepository.GetByCompanyIdAsync(commission.CompanyId, cancellationToken);
            return ApiResponse<CommissionResponseDto>.SuccessResponse(MapToDto(detail!), "Comisión actualizada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar comisión: {Id}", id);
            return ApiResponse<CommissionResponseDto>.ErrorResponse("Error al actualizar la comisión.");
        }
    }

    private static CommissionResponseDto MapToDto(Commission c) => new()
    {
        Id = c.Id, CompanyId = c.CompanyId, CompanyName = c.Company?.Name ?? string.Empty,
        Percentage = c.Percentage, MinAmount = c.MinAmount, MaxAmount = c.MaxAmount,
        IsActive = c.IsActive, CreatedAt = c.CreatedAt
    };
}
