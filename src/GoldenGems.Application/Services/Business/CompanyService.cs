using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using GoldenGems.Application.Interfaces.Business;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Business;

public class CompanyService : BaseService, ICompanyService
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyService(ICompanyRepository companyRepository, ILogger<CompanyService> logger) : base(logger)
    {
        _companyRepository = companyRepository;
    }

    public async Task<ApiResponse<CompanyResponseDto>> RegisterAsync(CreateCompanyRequestDto request, Guid ownerId, CancellationToken cancellationToken)
    {
        try
        {
            if (await _companyRepository.ExistsByNameAsync(request.Name, cancellationToken))
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Ya existe una empresa con ese nombre.");

            if (await _companyRepository.ExistsByNitAsync(request.NIT, cancellationToken))
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Ya existe una empresa con ese NIT.");

            var company = new Company
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                Logo = request.Logo?.Trim() ?? string.Empty,
                NIT = request.NIT.Trim(),
                Phone = request.Phone?.Trim() ?? string.Empty,
                Email = request.Email?.Trim() ?? string.Empty,
                WhatsAppNumber = request.WhatsAppNumber?.Trim() ?? string.Empty,
                IsDefault = false,
                OwnerId = ownerId
            };

            var created = await _companyRepository.CreateAsync(company, cancellationToken);
            _logger.LogInformation("Empresa creada: {Name} (ID: {Id})", created.Name, created.Id);
            return ApiResponse<CompanyResponseDto>.SuccessResponse(MapToDto(created), "Empresa registrada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar empresa");
            return ApiResponse<CompanyResponseDto>.ErrorResponse("Error al registrar la empresa.");
        }
    }

    public async Task<ApiResponse<List<CompanyResponseDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        try
        {
            var companies = await _companyRepository.GetAllActiveAsync(cancellationToken);
            return ApiResponse<List<CompanyResponseDto>>.SuccessResponse(
                companies.Select(MapToDto).ToList(),
                $"Se encontraron {companies.Count} empresas");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empresas");
            return ApiResponse<List<CompanyResponseDto>>.ErrorResponse("Error al obtener las empresas.");
        }
    }

    public async Task<ApiResponse<CompanyResponseDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyRepository.GetByIdAsync(id, cancellationToken);
            if (company == null || !company.IsActive)
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Empresa no encontrada.");

            return ApiResponse<CompanyResponseDto>.SuccessResponse(MapToDto(company), "Empresa encontrada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener empresa: {Id}", id);
            return ApiResponse<CompanyResponseDto>.ErrorResponse("Error al obtener la empresa.");
        }
    }

    public async Task<ApiResponse<CompanyResponseDto>> UpdateAsync(Guid id, CreateCompanyRequestDto request, Guid requesterId, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyRepository.GetByIdAsync(id, cancellationToken);
            if (company == null || !company.IsActive)
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Empresa no encontrada.");

            if (!string.Equals(company.Name, request.Name.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                if (await _companyRepository.ExistsByNameAsync(request.Name, cancellationToken))
                    return ApiResponse<CompanyResponseDto>.ErrorResponse("Ya existe una empresa con ese nombre.");
            }

            company.Name = request.Name.Trim();
            company.Description = request.Description?.Trim() ?? string.Empty;
            company.Logo = request.Logo?.Trim() ?? string.Empty;
            company.NIT = request.NIT.Trim();
            company.Phone = request.Phone?.Trim() ?? string.Empty;
            company.Email = request.Email?.Trim() ?? string.Empty;
            company.WhatsAppNumber = request.WhatsAppNumber?.Trim() ?? string.Empty;

            var updated = await _companyRepository.UpdateAsync(company, cancellationToken);
            return ApiResponse<CompanyResponseDto>.SuccessResponse(MapToDto(updated), "Empresa actualizada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar empresa: {Id}", id);
            return ApiResponse<CompanyResponseDto>.ErrorResponse("Error al actualizar la empresa.");
        }
    }

    public async Task<ApiResponse<CompanyResponseDto>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyRepository.GetByIdAsync(id, cancellationToken);
            if (company == null || !company.IsActive)
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Empresa no encontrada.");

            var deleted = await _companyRepository.DeleteAsync(company, cancellationToken);
            return ApiResponse<CompanyResponseDto>.SuccessResponse(MapToDto(deleted), "Empresa eliminada exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar empresa: {Id}", id);
            return ApiResponse<CompanyResponseDto>.ErrorResponse("Error al eliminar la empresa.");
        }
    }

    private static CompanyResponseDto MapToDto(Company c) => new()
    {
        Id = c.Id, Name = c.Name, Description = c.Description, Logo = c.Logo,
        NIT = c.NIT, Phone = c.Phone, Email = c.Email, WhatsAppNumber = c.WhatsAppNumber,
        IsDefault = c.IsDefault, OwnerId = c.OwnerId, IsActive = c.IsActive, CreatedAt = c.CreatedAt
    };
}
