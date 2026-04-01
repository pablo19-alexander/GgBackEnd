using FluentAssertions;
using GoldenGems.Application.DTOs.Auth;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Application.Models;
using GoldenGems.Application.Services.Auth;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace GoldenGems.Application.Tests.Services.Auth;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IPasswordHasherService> _passwordHasherMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _personRepositoryMock = new Mock<IPersonRepository>();
        _tokenServiceMock = new Mock<ITokenService>();
        _passwordHasherMock = new Mock<IPasswordHasherService>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        _sut = new AuthService(
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _personRepositoryMock.Object,
            _tokenServiceMock.Object,
            _passwordHasherMock.Object,
            _loggerMock.Object
        );
    }

    #region RegisterAsync

    [Fact]
    public async Task RegisterAsync_ConDatosValidos_DebeRetornarExito()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "Password123!",
            FirstName = "Juan",
            FirstLastName = "Pérez"
        };

        var userId = Guid.NewGuid();
        var createdUser = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            PasswordHash = "hashed_password"
        };

        var userWithRoles = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            UserRoles = new List<UserRole>()
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);
        _userRepositoryMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(userWithRoles);

        _passwordHasherMock.Setup(x => x.HashPassword(request.Password))
            .Returns("hashed_password");

        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new TokenResult("jwt_token_123", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Usuario registrado correctamente.");
        result.Data.Should().NotBeNull();
        result.Data!.Email.Should().Be(request.Email);
        result.Data.Username.Should().Be(request.Username);
        result.Data.Token.Should().Be("jwt_token_123");
    }

    [Fact]
    public async Task RegisterAsync_ConEmailExistente_DebeRetornarError()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "existing@example.com",
            Username = "testuser",
            Password = "Password123!",
            FirstName = "Juan",
            FirstLastName = "Pérez"
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("El correo electrónico ya está registrado.");
        _userRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterAsync_ConUsernameExistente_DebeRetornarError()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Username = "existinguser",
            Password = "Password123!",
            FirstName = "Juan",
            FirstLastName = "Pérez"
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync("existinguser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("El nombre de usuario ya está en uso.");
    }

    [Fact]
    public async Task RegisterAsync_ConRoles_DebeAsignarRoles()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var request = new RegisterRequestDto
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "Password123!",
            FirstName = "Juan",
            FirstLastName = "Pérez",
            RoleIds = new List<Guid> { roleId }
        };

        var createdUser = new User { Id = userId, Email = request.Email, Username = request.Username };
        var userWithRoles = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            UserRoles = new List<UserRole>
            {
                new() { RoleId = roleId, Role = new Role { Name = "User" } }
            }
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);
        _userRepositoryMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(userWithRoles);
        _roleRepositoryMock.Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { roleId });
        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed");
        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new TokenResult("token", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.AssignRolesAsync(userId, It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_DebeNormalizarEmailYUsername()
    {
        // Arrange
        var request = new RegisterRequestDto
        {
            Email = "  Test@Example.com  ",
            Username = "  TestUser  ",
            Password = "Password123!",
            FirstName = "Juan",
            FirstLastName = "Pérez"
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync("Test@Example.com", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync("TestUser", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken _) => u);
        _userRepositoryMock.Setup(x => x.GetByIdWithRolesAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { UserRoles = new List<UserRole>() });
        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed");
        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new TokenResult("token", DateTime.UtcNow.AddHours(1)));

        // Act
        await _sut.RegisterAsync(request);

        // Assert
        _userRepositoryMock.Verify(x => x.EmailExistsAsync("Test@Example.com", It.IsAny<CancellationToken>()));
        _userRepositoryMock.Verify(x => x.UsernameExistsAsync("TestUser", It.IsAny<CancellationToken>()));
    }

    #endregion

    #region LoginAsync

    [Fact]
    public async Task LoginAsync_ConCredencialesValidas_DebeRetornarToken()
    {
        // Arrange
        var request = new LoginRequestDto { Identifier = "testuser", Password = "Password123!" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            Username = "testuser",
            PasswordHash = "hashed_password",
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "User" } }
            }
        };

        _userRepositoryMock.Setup(x => x.GetByIdentifierWithRolesAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        bool rehashNeeded = false;
        _passwordHasherMock.Setup(x => x.VerifyPassword("hashed_password", "Password123!", out rehashNeeded))
            .Returns(true);

        _tokenServiceMock.Setup(x => x.GenerateToken(user, It.IsAny<IEnumerable<string>>()))
            .Returns(new TokenResult("jwt_token", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Autenticación exitosa.");
        result.Data!.Token.Should().Be("jwt_token");
        result.Data.Username.Should().Be("testuser");
    }

    [Fact]
    public async Task LoginAsync_ConUsuarioInexistente_DebeRetornarError()
    {
        // Arrange
        var request = new LoginRequestDto { Identifier = "noexiste", Password = "Password123!" };

        _userRepositoryMock.Setup(x => x.GetByIdentifierWithRolesAsync("noexiste", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Credenciales inválidas.");
    }

    [Fact]
    public async Task LoginAsync_ConPasswordIncorrecto_DebeRetornarError()
    {
        // Arrange
        var request = new LoginRequestDto { Identifier = "testuser", Password = "WrongPassword" };
        var user = new User
        {
            Username = "testuser",
            PasswordHash = "hashed_password",
            UserRoles = new List<UserRole>()
        };

        _userRepositoryMock.Setup(x => x.GetByIdentifierWithRolesAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        bool rehashNeeded = false;
        _passwordHasherMock.Setup(x => x.VerifyPassword("hashed_password", "WrongPassword", out rehashNeeded))
            .Returns(false);

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Credenciales inválidas.");
    }

    [Fact]
    public async Task LoginAsync_CuandoNecesitaRehash_DebeActualizarPassword()
    {
        // Arrange
        var request = new LoginRequestDto { Identifier = "testuser", Password = "Password123!" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = "old_hash",
            UserRoles = new List<UserRole>()
        };

        _userRepositoryMock.Setup(x => x.GetByIdentifierWithRolesAsync("testuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        bool rehashNeeded = true;
        _passwordHasherMock.Setup(x => x.VerifyPassword("old_hash", "Password123!", out rehashNeeded))
            .Returns(true);
        _passwordHasherMock.Setup(x => x.HashPassword("Password123!"))
            .Returns("new_hash");

        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new TokenResult("token", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _sut.LoginAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        _userRepositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(u => u.PasswordHash == "new_hash"), It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region CreateUserAsync

    [Fact]
    public async Task CreateUserAsync_ConDatosValidos_DebeCrearUsuarioYPersona()
    {
        // Arrange
        var roleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var docTypeId = Guid.NewGuid();

        var request = new CreateUserRequestDto
        {
            Email = "admin@example.com",
            Username = "adminuser",
            Password = "Admin1234!",
            FirstName = "Admin",
            FirstLastName = "Sistema",
            DocumentTypeId = docTypeId,
            DocumentNumber = "12345678",
            RoleIds = new List<Guid> { roleId },
            IsActive = true
        };

        var createdUser = new User { Id = userId, Email = request.Email, Username = request.Username };
        var userWithRoles = new User
        {
            Id = userId,
            Email = request.Email,
            Username = request.Username,
            UserRoles = new List<UserRole>
            {
                new() { Role = new Role { Name = "Admin" } }
            }
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(createdUser);
        _userRepositoryMock.Setup(x => x.GetByIdWithRolesAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(userWithRoles);
        _roleRepositoryMock.Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid> { roleId });
        _passwordHasherMock.Setup(x => x.HashPassword(It.IsAny<string>())).Returns("hashed");
        _tokenServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .Returns(new TokenResult("admin_token", DateTime.UtcNow.AddHours(1)));

        // Act
        var result = await _sut.CreateUserAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Usuario creado exitosamente.");
        result.Data!.Token.Should().Be("admin_token");

        _personRepositoryMock.Verify(x => x.CreateAsync(
            It.Is<GoldenGems.Domain.Entities.People.Person>(p =>
                p.FirstName == "Admin" &&
                p.FirstLastName == "Sistema" &&
                p.DocumentNumber == "12345678" &&
                p.UserId == userId),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_SinRoles_DebeRetornarError()
    {
        // Arrange
        var request = new CreateUserRequestDto
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "Password123!",
            FirstName = "Test",
            FirstLastName = "User",
            DocumentTypeId = Guid.NewGuid(),
            DocumentNumber = "12345678",
            RoleIds = new List<Guid>()
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var result = await _sut.CreateUserAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Debe asignar al menos un rol.");
    }

    [Fact]
    public async Task CreateUserAsync_ConRolesInvalidos_DebeRetornarError()
    {
        // Arrange
        var request = new CreateUserRequestDto
        {
            Email = "test@example.com",
            Username = "testuser",
            Password = "Password123!",
            FirstName = "Test",
            FirstLastName = "User",
            DocumentTypeId = Guid.NewGuid(),
            DocumentNumber = "12345678",
            RoleIds = new List<Guid> { Guid.NewGuid() }
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _userRepositoryMock.Setup(x => x.UsernameExistsAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _roleRepositoryMock.Setup(x => x.GetExistingRoleIdsAsync(It.IsAny<IEnumerable<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Guid>());

        // Act
        var result = await _sut.CreateUserAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Ninguno de los roles proporcionados es válido.");
    }

    [Fact]
    public async Task CreateUserAsync_ConEmailDuplicado_DebeRetornarError()
    {
        // Arrange
        var request = new CreateUserRequestDto
        {
            Email = "existing@example.com",
            Username = "newuser",
            Password = "Password123!",
            FirstName = "Test",
            FirstLastName = "User",
            DocumentTypeId = Guid.NewGuid(),
            DocumentNumber = "12345678",
            RoleIds = new List<Guid> { Guid.NewGuid() }
        };

        _userRepositoryMock.Setup(x => x.EmailExistsAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreateUserAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("El correo electrónico ya está registrado.");
    }

    #endregion
}
