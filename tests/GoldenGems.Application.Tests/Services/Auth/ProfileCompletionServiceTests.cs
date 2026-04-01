using FluentAssertions;
using GoldenGems.Application.Services.Auth;
using GoldenGems.Domain.Entities.People;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace GoldenGems.Application.Tests.Services.Auth;

public class ProfileCompletionServiceTests
{
    private readonly Mock<IPersonRepository> _personRepositoryMock;
    private readonly ProfileCompletionService _sut;

    public ProfileCompletionServiceTests()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();
        var loggerMock = new Mock<ILogger<ProfileCompletionService>>();
        _sut = new ProfileCompletionService(_personRepositoryMock.Object, loggerMock.Object);
    }

    #region IsProfileCompleteAsync

    [Fact]
    public async Task IsProfileCompleteAsync_SinPersona_DebeRetornarFalse()
    {
        _personRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        var result = await _sut.IsProfileCompleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsProfileCompleteAsync_ConDatosCompletos_DebeRetornarTrue()
    {
        var person = new Person
        {
            FirstName = "Pablo",
            FirstLastName = "Salazar",
            DocumentNumber = "12345678",
            DocumentTypeId = Guid.NewGuid()
        };

        _personRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        var result = await _sut.IsProfileCompleteAsync(Guid.NewGuid());

        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Salazar", "12345678")]
    [InlineData("Pablo", "", "12345678")]
    [InlineData("Pablo", "Salazar", "")]
    public async Task IsProfileCompleteAsync_ConCamposFaltantes_DebeRetornarFalse(
        string firstName, string lastName, string docNumber)
    {
        var person = new Person
        {
            FirstName = firstName,
            FirstLastName = lastName,
            DocumentNumber = docNumber,
            DocumentTypeId = Guid.NewGuid()
        };

        _personRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        var result = await _sut.IsProfileCompleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsProfileCompleteAsync_ConDocumentTypeVacio_DebeRetornarFalse()
    {
        var person = new Person
        {
            FirstName = "Pablo",
            FirstLastName = "Salazar",
            DocumentNumber = "12345678",
            DocumentTypeId = Guid.Empty
        };

        _personRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        var result = await _sut.IsProfileCompleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    #endregion

    #region ValidateProfileOrError

    [Fact]
    public async Task ValidateProfileOrError_ConPerfilCompleto_DebeRetornarNull()
    {
        var person = new Person
        {
            FirstName = "Pablo",
            FirstLastName = "Salazar",
            DocumentNumber = "12345678",
            DocumentTypeId = Guid.NewGuid()
        };

        _personRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(person);

        var result = await _sut.ValidateProfileOrError<string>(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidateProfileOrError_ConPerfilIncompleto_DebeRetornarApiResponseError()
    {
        _personRepositoryMock.Setup(x => x.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Person?)null);

        var result = await _sut.ValidateProfileOrError<string>(Guid.NewGuid());

        result.Should().NotBeNull();
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("completar tu perfil");
    }

    #endregion
}
