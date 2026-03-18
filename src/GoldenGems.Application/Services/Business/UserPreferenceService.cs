using System.Text.Json;
using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using GoldenGems.Application.Interfaces.Business;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Business;

public class UserPreferenceService : BaseService, IUserPreferenceService
{
    private readonly IUserPreferenceRepository _preferenceRepository;

    public UserPreferenceService(IUserPreferenceRepository preferenceRepository, ILogger<UserPreferenceService> logger) : base(logger)
    {
        _preferenceRepository = preferenceRepository;
    }

    public async Task<ApiResponse<PreferencesResponseDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var pref = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
            if (pref == null)
                return ApiResponse<PreferencesResponseDto>.SuccessResponse(new PreferencesResponseDto { ShowAllCompanies = true }, "Sin preferencias configuradas.");

            return ApiResponse<PreferencesResponseDto>.SuccessResponse(MapToDto(pref), "Preferencias encontradas.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener preferencias: {UserId}", userId);
            return ApiResponse<PreferencesResponseDto>.ErrorResponse("Error al obtener las preferencias.");
        }
    }

    public async Task<ApiResponse<PreferencesResponseDto>> UpdateAsync(Guid userId, UpdatePreferencesRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);

            if (existing == null)
            {
                var pref = new UserPreference
                {
                    UserId = userId,
                    PreferredCompanyId = request.PreferredCompanyId,
                    PreferredCategories = JsonSerializer.Serialize(request.PreferredCategories),
                    ShowAllCompanies = request.ShowAllCompanies
                };
                var created = await _preferenceRepository.CreateAsync(pref, cancellationToken);
                var result = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
                return ApiResponse<PreferencesResponseDto>.SuccessResponse(MapToDto(result!), "Preferencias creadas exitosamente.");
            }

            existing.PreferredCompanyId = request.PreferredCompanyId;
            existing.PreferredCategories = JsonSerializer.Serialize(request.PreferredCategories);
            existing.ShowAllCompanies = request.ShowAllCompanies;
            await _preferenceRepository.UpdateAsync(existing, cancellationToken);

            var updated = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
            return ApiResponse<PreferencesResponseDto>.SuccessResponse(MapToDto(updated!), "Preferencias actualizadas exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar preferencias: {UserId}", userId);
            return ApiResponse<PreferencesResponseDto>.ErrorResponse("Error al actualizar las preferencias.");
        }
    }

    public async Task<ApiResponse<PreferencesResponseDto>> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var existing = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
            if (existing == null)
                return ApiResponse<PreferencesResponseDto>.ErrorResponse("No hay preferencias configuradas.");

            await _preferenceRepository.DeleteAsync(existing, cancellationToken);
            return ApiResponse<PreferencesResponseDto>.SuccessResponse(MapToDto(existing), "Preferencias eliminadas.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar preferencias: {UserId}", userId);
            return ApiResponse<PreferencesResponseDto>.ErrorResponse("Error al eliminar las preferencias.");
        }
    }

    private static PreferencesResponseDto MapToDto(UserPreference pref)
    {
        var categories = new List<Guid>();
        if (!string.IsNullOrWhiteSpace(pref.PreferredCategories))
        {
            try { categories = JsonSerializer.Deserialize<List<Guid>>(pref.PreferredCategories) ?? new(); } catch { }
        }

        return new PreferencesResponseDto
        {
            Id = pref.Id,
            PreferredCompanyId = pref.PreferredCompanyId,
            PreferredCompanyName = pref.PreferredCompany?.Name ?? string.Empty,
            PreferredCategories = categories,
            ShowAllCompanies = pref.ShowAllCompanies
        };
    }
}
