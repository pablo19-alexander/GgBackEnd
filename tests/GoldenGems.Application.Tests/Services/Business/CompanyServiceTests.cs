using FluentAssertions;
using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Business;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Application.Interfaces.Business;
using GoldenGems.Application.Services.Business;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace GoldenGems.Application.Tests.Services.Business;

public class CompanyServiceTests
{
    private readonly Mock<ICompanyRepository> _companyRepositoryMock;
    private readonly Mock<IProfileCompletionService> _profileServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IImageStorageService> _storageServiceMock;
    private readonly Mock<ILogger<CompanyService>> _loggerMock;
    private readonly CompanyService _sut;

    public CompanyServiceTests()
    {
        _companyRepositoryMock = new Mock<ICompanyRepository>();
        _profileServiceMock = new Mock<IProfileCompletionService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _storageServiceMock = new Mock<IImageStorageService>();
        _loggerMock = new Mock<ILogger<CompanyService>>();
        _sut = new CompanyService(
            _companyRepositoryMock.Object,
            _profileServiceMock.Object,
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _storageServiceMock.Object,
            _loggerMock.Object);

        // Por defecto, perfil completo
        _profileServiceMock
            .Setup(x => x.ValidateProfileOrError<CompanyResponseDto>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApiResponse<CompanyResponseDto>)null!);
    }

    #region RegisterAsync - Validacion de perfil

    [Fact]
    public async Task RegisterAsync_ConPerfilIncompleto_DebeRetornarError()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var request = new CreateCompanyRequestDto { Name = "Test", NIT = "123456789" };

        _profileServiceMock
            .Setup(x => x.ValidateProfileOrError<CompanyResponseDto>(ownerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<CompanyResponseDto>.ErrorResponse("Debes completar tu perfil antes de continuar."));

        // Act
        var result = await _sut.RegisterAsync(request, ownerId, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("completar tu perfil");
        _companyRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region RegisterAsync

    [Fact]
    public async Task RegisterAsync_ConDatosValidos_DebeCrearEmpresa()
    {
        // Arrange
        var ownerId = Guid.NewGuid();
        var request = new CreateCompanyRequestDto
        {
            Name = "Golden Gems SAS",
            NIT = "900123456-7",
            Description = "Joyería de alta gama",
            Phone = "3001234567",
            Email = "info@goldengems.com"
        };

        _companyRepositoryMock.Setup(x => x.ExistsByNameAsync("Golden Gems SAS", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _companyRepositoryMock.Setup(x => x.ExistsByNitAsync("900123456-7", It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _companyRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company c, CancellationToken _) => c);

        // Act
        var result = await _sut.RegisterAsync(request, ownerId, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Golden Gems SAS");
        result.Data.NIT.Should().Be("900123456-7");
        result.Data.OwnerId.Should().Be(ownerId);
        result.Data.IsDefault.Should().BeFalse();
    }

    [Fact]
    public async Task RegisterAsync_ConNombreDuplicado_DebeRetornarError()
    {
        // Arrange
        var request = new CreateCompanyRequestDto { Name = "Existing Company", NIT = "123456789" };

        _companyRepositoryMock.Setup(x => x.ExistsByNameAsync("Existing Company", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var result = await _sut.RegisterAsync(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("nombre");
    }

    [Fact]
    public async Task RegisterAsync_ConNitDuplicado_DebeRetornarError()
    {
        // Arrange
        var request = new CreateCompanyRequestDto { Name = "New Company", NIT = "900123456-7" };

        _companyRepositoryMock.Setup(x => x.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _companyRepositoryMock.Setup(x => x.ExistsByNitAsync("900123456-7", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _sut.RegisterAsync(request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("NIT");
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ConIdExistente_DebeRetornarEmpresa()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var company = new Company
        {
            Id = companyId,
            Name = "Golden Gems",
            NIT = "900123456-7",
            IsActive = true
        };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        // Act
        var result = await _sut.GetByIdAsync(companyId, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Golden Gems");
    }

    [Fact]
    public async Task GetByIdAsync_ConIdInexistente_DebeRetornarError()
    {
        // Arrange
        _companyRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _sut.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("no encontrada");
    }

    [Fact]
    public async Task GetByIdAsync_ConEmpresaInactiva_DebeRetornarError()
    {
        // Arrange
        var company = new Company { Id = Guid.NewGuid(), Name = "Inactive", IsActive = false };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(company.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        // Act
        var result = await _sut.GetByIdAsync(company.Id, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ConIdExistente_DebeEliminar()
    {
        // Arrange
        var company = new Company { Id = Guid.NewGuid(), Name = "To Delete", IsActive = true };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(company.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);
        _companyRepositoryMock.Setup(x => x.DeleteAsync(company, It.IsAny<CancellationToken>()))
            .ReturnsAsync(company);

        // Act
        var result = await _sut.DeleteAsync(company.Id, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Contain("eliminada");
    }

    [Fact]
    public async Task DeleteAsync_ConIdInexistente_DebeRetornarError()
    {
        // Arrange
        _companyRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        // Act
        var result = await _sut.DeleteAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ConNombreCambiado_DebeValidarUnicidad()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var existing = new Company { Id = companyId, Name = "Old Name", NIT = "123", IsActive = true };
        var request = new CreateCompanyRequestDto { Name = "New Name", NIT = "456" };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _companyRepositoryMock.Setup(x => x.ExistsByNameAsync("New Name", It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var result = await _sut.UpdateAsync(companyId, request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("nombre");
    }

    [Fact]
    public async Task UpdateAsync_ConMismoNombre_NoDebeValidarUnicidad()
    {
        // Arrange
        var companyId = Guid.NewGuid();
        var existing = new Company { Id = companyId, Name = "Same Name", NIT = "123", IsActive = true };
        var request = new CreateCompanyRequestDto { Name = "Same Name", NIT = "456" };

        _companyRepositoryMock.Setup(x => x.GetByIdAsync(companyId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _companyRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Company>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company c, CancellationToken _) => c);

        // Act
        var result = await _sut.UpdateAsync(companyId, request, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        _companyRepositoryMock.Verify(x => x.ExistsByNameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion
}
