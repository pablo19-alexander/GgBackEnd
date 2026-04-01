using FluentAssertions;
using GoldenGems.Application.DTOs.Admin;
using GoldenGems.Application.Services.Admin;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace GoldenGems.Application.Tests.Services.Admin;

public class RoleServiceTests
{
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<ILogger<RoleService>> _loggerMock;
    private readonly RoleService _sut;

    public RoleServiceTests()
    {
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _loggerMock = new Mock<ILogger<RoleService>>();
        _sut = new RoleService(_roleRepositoryMock.Object, _loggerMock.Object);
    }

    #region CreateRoleAsync

    [Fact]
    public async Task CreateRoleAsync_ConDatosValidos_DebeCrearRol()
    {
        // Arrange
        var request = new CreateRoleRequestDto { Name = "Admin", Description = "Administrador" };

        _roleRepositoryMock.Setup(x => x.ExistsByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _roleRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Role r, CancellationToken _) => r);

        // Act
        var result = await _sut.CreateRoleAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Admin");
        result.Data.Description.Should().Be("Administrador");
        result.Data.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task CreateRoleAsync_ConNombreDuplicado_DebeRetornarError()
    {
        // Arrange
        var request = new CreateRoleRequestDto { Name = "Admin" };

        _roleRepositoryMock.Setup(x => x.ExistsByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.CreateRoleAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Ya existe un rol");
        _roleRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Role>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateRoleAsync_ConNombreVacio_DebeRetornarError()
    {
        // Arrange
        var request = new CreateRoleRequestDto { Name = "" };

        // Act
        var result = await _sut.CreateRoleAsync(request, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("El nombre del rol es requerido");
    }

    [Fact]
    public async Task CreateRoleAsync_ConRequestNulo_DebeRetornarError()
    {
        // Act
        var result = await _sut.CreateRoleAsync(null!, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("La solicitud es nula");
    }

    #endregion

    #region GetAllRolesAsync

    [Fact]
    public async Task GetAllRolesAsync_DebeRetornarTodosLosRoles()
    {
        // Arrange
        var roles = new List<Role>
        {
            new() { Id = Guid.NewGuid(), Name = "Admin", IsActive = true },
            new() { Id = Guid.NewGuid(), Name = "User", IsActive = true }
        };

        _roleRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _sut.GetAllRolesAsync(CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Message.Should().Contain("2 roles");
    }

    #endregion

    #region RoleExistsByNameAsync

    [Fact]
    public async Task RoleExistsByNameAsync_ConNombreVacio_DebeRetornarFalse()
    {
        var result = await _sut.RoleExistsByNameAsync("", CancellationToken.None);
        result.Should().BeFalse();
    }

    [Fact]
    public async Task RoleExistsByNameAsync_ConNombreExistente_DebeRetornarTrue()
    {
        _roleRepositoryMock.Setup(x => x.ExistsByNameAsync("Admin", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _sut.RoleExistsByNameAsync("Admin", CancellationToken.None);
        result.Should().BeTrue();
    }

    #endregion

    #region GetRoleByIdAsync

    [Fact]
    public async Task GetRoleByIdAsync_ConGuidVacio_DebeRetornarNull()
    {
        var result = await _sut.GetRoleByIdAsync(Guid.Empty, CancellationToken.None);
        result.Should().BeNull();
    }

    #endregion
}
