using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Auth;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace GoldenGems.Application.Services.Auth;

public class AuthService : BaseService, IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPersonRepository _personRepository;
    private readonly ITokenService _tokenService;
    private readonly IPasswordHasherService _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPersonRepository personRepository,
        ITokenService tokenService,
        IPasswordHasherService passwordHasher,
        ILogger<AuthService> logger) : base(logger)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _personRepository = personRepository;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<AuthResponseDto>> RegisterAsync(RegisterRequestDto request, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = request.Email.Trim();
        var normalizedUsername = request.Username.Trim();

        if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
            return ApiResponse<AuthResponseDto>.ErrorResponse("El correo electrónico ya está registrado.");

        if (await _userRepository.UsernameExistsAsync(normalizedUsername, cancellationToken))
            return ApiResponse<AuthResponseDto>.ErrorResponse("El nombre de usuario ya está en uso.");

        var user = new User
        {
            Email = normalizedEmail,
            Username = normalizedUsername
        };

        user.PasswordHash = _passwordHasher.HashPassword(request.Password);

        var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

        // Asignar roles
        if (request.RoleIds?.Any() == true)
        {
            var existingRoleIds = await _roleRepository.GetExistingRoleIdsAsync(request.RoleIds, cancellationToken);
            if (existingRoleIds.Any())
            {
                await _userRepository.AssignRolesAsync(createdUser.Id, existingRoleIds, cancellationToken);
            }
        }

        var userWithRoles = await _userRepository.GetByIdWithRolesAsync(createdUser.Id, cancellationToken);
        return BuildAuthSuccessResponse(userWithRoles!, "Usuario registrado correctamente.");
    }

    public async Task<ApiResponse<AuthResponseDto>> LoginAsync(LoginRequestDto request, CancellationToken cancellationToken = default)
    {
        var identifier = request.Identifier.Trim();

        var user = await _userRepository.GetByIdentifierWithRolesAsync(identifier, cancellationToken);

        if (user == null)
            return ApiResponse<AuthResponseDto>.ErrorResponse("Credenciales inválidas.");

        var isValid = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password, out var rehashNeeded);
        if (!isValid)
            return ApiResponse<AuthResponseDto>.ErrorResponse("Credenciales inválidas.");

        if (rehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(request.Password);
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        return BuildAuthSuccessResponse(user, "Autenticación exitosa.");
    }

    public async Task<ApiResponse<AuthResponseDto>> CreateUserAsync(CreateUserRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var normalizedEmail = request.Email.Trim();
            var normalizedUsername = request.Username.Trim();

            if (await _userRepository.EmailExistsAsync(normalizedEmail, cancellationToken))
                return ApiResponse<AuthResponseDto>.ErrorResponse("El correo electrónico ya está registrado.");

            if (await _userRepository.UsernameExistsAsync(normalizedUsername, cancellationToken))
                return ApiResponse<AuthResponseDto>.ErrorResponse("El nombre de usuario ya está en uso.");

            if (request.RoleIds == null || !request.RoleIds.Any())
                return ApiResponse<AuthResponseDto>.ErrorResponse("Debe asignar al menos un rol.");

            var existingRoleIds = await _roleRepository.GetExistingRoleIdsAsync(request.RoleIds, cancellationToken);
            if (!existingRoleIds.Any())
                return ApiResponse<AuthResponseDto>.ErrorResponse("Ninguno de los roles proporcionados es válido.");

            var user = new User
            {
                Email = normalizedEmail,
                Username = normalizedUsername,
                IsActive = request.IsActive
            };

            user.PasswordHash = _passwordHasher.HashPassword(request.Password);

            var createdUser = await _userRepository.CreateAsync(user, cancellationToken);

            await _userRepository.AssignRolesAsync(createdUser.Id, existingRoleIds, cancellationToken);

            var person = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName.Trim(),
                SecondName = request.SecondName?.Trim() ?? string.Empty,
                FirstLastName = request.FirstLastName.Trim(),
                SecondLastName = request.SecondLastName?.Trim() ?? string.Empty,
                DocumentTypeId = request.DocumentTypeId,
                DocumentNumber = request.DocumentNumber.Trim(),
                UserId = createdUser.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _personRepository.CreateAsync(person, cancellationToken);

            var userWithRoles = await _userRepository.GetByIdWithRolesAsync(createdUser.Id, cancellationToken);

            _logger.LogInformation("Usuario creado por admin: {Username} (ID: {Id})", normalizedUsername, createdUser.Id);
            return BuildAuthSuccessResponse(userWithRoles!, "Usuario creado exitosamente.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear usuario por admin");
            return ApiResponse<AuthResponseDto>.ErrorResponse("Error al crear el usuario.");
        }
    }

    private ApiResponse<AuthResponseDto> BuildAuthSuccessResponse(User user, string message)
    {
        var roleNames = user.UserRoles
            .Select(ur => ur.Role?.Name ?? string.Empty)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var tokenResult = _tokenService.GenerateToken(user, roleNames);

        var payload = new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            Username = user.Username,
            Roles = roleNames,
            Token = tokenResult.Token,
            ExpiresAt = tokenResult.ExpiresAt
        };

        return ApiResponse<AuthResponseDto>.SuccessResponse(payload, message);
    }
}
