using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Application.Interfaces.Business;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Business;

public class CompanyService : BaseService, ICompanyService
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IProfileCompletionService _profileService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IImageStorageService _storageService;

    public CompanyService(
        ICompanyRepository companyRepository,
        IProfileCompletionService profileService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IImageStorageService storageService,
        ILogger<CompanyService> logger) : base(logger)
    {
        _companyRepository = companyRepository;
        _profileService = profileService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _storageService = storageService;
    }

    public async Task<ApiResponse<CompanyResponseDto>> AdminRegisterAsync(AdminCreateCompanyRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Validar que el usuario exista y esté activo
            var user = await _userRepository.GetByIdWithRolesAsync(request.OwnerId, cancellationToken);
            if (user == null || !user.IsActive)
                return ApiResponse<CompanyResponseDto>.ErrorResponse("El usuario propietario no existe o está inactivo.");

            // 2. Validar que exista el rol "Empresa" antes de crear la empresa
            var empresaRole = await _roleRepository.GetByNameAsync("Empresa", cancellationToken);
            if (empresaRole == null)
                return ApiResponse<CompanyResponseDto>.ErrorResponse("El rol 'Empresa' no existe. Créalo desde /admin/roles antes de registrar empresas.");

            // 3. Validar unicidad de Name y NIT
            if (await _companyRepository.ExistsByNameAsync(request.Name, cancellationToken))
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Ya existe una empresa con ese nombre.");

            if (await _companyRepository.ExistsByNitAsync(request.NIT, cancellationToken))
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Ya existe una empresa con ese NIT.");

            // 4. Si se marca como principal, desmarcar la anterior principal
            if (request.IsDefault)
            {
                var currentDefault = await _companyRepository.GetDefaultCompanyAsync(cancellationToken);
                if (currentDefault != null)
                {
                    currentDefault.IsDefault = false;
                    await _companyRepository.UpdateAsync(currentDefault, cancellationToken);
                    _logger.LogInformation("Empresa principal anterior desmarcada: {Id}", currentDefault.Id);
                }
            }

            // 5. Crear empresa
            var company = new Company
            {
                Name = request.Name.Trim(),
                Description = request.Description?.Trim() ?? string.Empty,
                Logo = request.Logo?.Trim() ?? string.Empty,
                NIT = request.NIT.Trim(),
                Phone = request.Phone?.Trim() ?? string.Empty,
                Email = request.Email?.Trim() ?? string.Empty,
                WhatsAppNumber = request.WhatsAppNumber?.Trim() ?? string.Empty,
                IsDefault = request.IsDefault,
                OwnerId = request.OwnerId
            };

            var created = await _companyRepository.CreateAsync(company, cancellationToken);

            // 6. Asignar rol Empresa al usuario si aún no lo tiene
            var alreadyHasRole = user.UserRoles.Any(ur => ur.IsActive && ur.RoleId == empresaRole.Id);
            if (!alreadyHasRole)
            {
                await _userRepository.AssignRolesAsync(request.OwnerId, new[] { empresaRole.Id }, cancellationToken);
                _logger.LogInformation("Rol 'Empresa' asignado al usuario {UserId}", request.OwnerId);
            }

            _logger.LogInformation("Empresa creada por admin: {Name} (ID: {Id}) para usuario {UserId}", created.Name, created.Id, request.OwnerId);
            return ApiResponse<CompanyResponseDto>.SuccessResponse(MapToDto(created), "Empresa registrada exitosamente. El usuario ahora tiene rol Empresa.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al registrar empresa (admin)");
            return ApiResponse<CompanyResponseDto>.ErrorResponse("Error al registrar la empresa.");
        }
    }

    public async Task<ApiResponse<CompanyResponseDto>> RegisterAsync(CreateCompanyRequestDto request, Guid ownerId, CancellationToken cancellationToken)
    {
        try
        {
            var profileError = await _profileService.ValidateProfileOrError<CompanyResponseDto>(ownerId, cancellationToken);
            if (profileError != null)
                return profileError;

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

    public async Task<ApiResponse<CompanyResponseDto>> UploadLogoAsync(Guid id, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            var company = await _companyRepository.GetByIdAsync(id, cancellationToken);
            if (company == null || !company.IsActive)
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Empresa no encontrada.");

            if (file == null || file.Length == 0)
                return ApiResponse<CompanyResponseDto>.ErrorResponse("El archivo está vacío.");

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp", "image/gif" };
            if (!allowedTypes.Contains(file.ContentType.ToLower()))
                return ApiResponse<CompanyResponseDto>.ErrorResponse("Formato de imagen no permitido. Use: JPG, PNG, WEBP o GIF.");

            if (!string.IsNullOrWhiteSpace(company.Logo))
            {
                try { await _storageService.DeleteAsync(company.Logo, cancellationToken); }
                catch (Exception ex) { _logger.LogWarning(ex, "No se pudo eliminar logo anterior: {Logo}", company.Logo); }
            }

            var url = await _storageService.UploadAsync(file, "companies", cancellationToken);
            company.Logo = url;
            var updated = await _companyRepository.UpdateAsync(company, cancellationToken);

            _logger.LogInformation("Logo subido para empresa {Id}: {Url}", id, url);
            return ApiResponse<CompanyResponseDto>.SuccessResponse(MapToDto(updated), "Logo actualizado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al subir logo de empresa: {Id}", id);
            return ApiResponse<CompanyResponseDto>.ErrorResponse("Error al subir el logo.");
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
