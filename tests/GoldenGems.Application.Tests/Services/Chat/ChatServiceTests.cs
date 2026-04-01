using FluentAssertions;
using GoldenGems.Application.Common;
using GoldenGems.Application.DTOs.Chat;
using GoldenGems.Application.Interfaces.Auth;
using GoldenGems.Application.Services.Chat;
using GoldenGems.Domain.Entities.Business;
using GoldenGems.Domain.Entities.Chat;
using GoldenGems.Domain.Entities.Security;
using GoldenGems.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace GoldenGems.Application.Tests.Services.Chat;

public class ChatServiceTests
{
    private readonly Mock<IConversationRepository> _conversationRepoMock;
    private readonly Mock<IMessageRepository> _messageRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<ICompanyRepository> _companyRepoMock;
    private readonly Mock<IProfileCompletionService> _profileServiceMock;
    private readonly ChatService _sut;

    public ChatServiceTests()
    {
        _conversationRepoMock = new Mock<IConversationRepository>();
        _messageRepoMock = new Mock<IMessageRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _companyRepoMock = new Mock<ICompanyRepository>();
        _profileServiceMock = new Mock<IProfileCompletionService>();
        var loggerMock = new Mock<ILogger<ChatService>>();

        _sut = new ChatService(
            _conversationRepoMock.Object,
            _messageRepoMock.Object,
            _productRepoMock.Object,
            _companyRepoMock.Object,
            _profileServiceMock.Object,
            loggerMock.Object
        );

        // Por defecto, perfil completo
        _profileServiceMock
            .Setup(x => x.ValidateProfileOrError<ConversationResponseDto>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApiResponse<ConversationResponseDto>)null!);
        _profileServiceMock
            .Setup(x => x.ValidateProfileOrError<MessageResponseDto>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ApiResponse<MessageResponseDto>)null!);
    }

    #region StartConversationAsync - Validacion de perfil

    [Fact]
    public async Task StartConversationAsync_ConPerfilIncompleto_DebeRetornarError()
    {
        // Arrange
        var buyerId = Guid.NewGuid();
        _profileServiceMock
            .Setup(x => x.ValidateProfileOrError<ConversationResponseDto>(buyerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<ConversationResponseDto>.ErrorResponse("Debes completar tu perfil antes de continuar."));

        // Act
        var result = await _sut.StartConversationAsync(buyerId, Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("completar tu perfil");
        _productRepoMock.Verify(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task StartConversationAsync_ConPerfilCompleto_DebeContinuarFlujo()
    {
        // Arrange
        var buyerId = Guid.NewGuid();
        var sellerId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        var product = new Product
        {
            Id = productId,
            Name = "Anillo",
            IsNegotiable = true,
            CompanyId = Guid.NewGuid(),
            Company = new Company { OwnerId = sellerId }
        };

        _productRepoMock.Setup(x => x.GetByIdWithDetailsAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _conversationRepoMock.Setup(x => x.GetActiveByBuyerAndProductAsync(buyerId, productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);
        _conversationRepoMock.Setup(x => x.CreateAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation c, CancellationToken _) => c);
        _conversationRepoMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation
            {
                BuyerId = buyerId,
                SellerId = sellerId,
                ProductId = productId,
                Buyer = new User { Username = "buyer" },
                Seller = new User { Username = "seller" },
                Product = product,
                Company = product.Company,
                Status = ConversationStatus.Open
            });

        // Act
        var result = await _sut.StartConversationAsync(buyerId, productId, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().Be("Conversación iniciada.");
    }

    #endregion

    #region OfferPriceAsync - Validacion de perfil

    [Fact]
    public async Task OfferPriceAsync_ConPerfilIncompleto_DebeRetornarError()
    {
        // Arrange
        var senderId = Guid.NewGuid();
        _profileServiceMock
            .Setup(x => x.ValidateProfileOrError<MessageResponseDto>(senderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<MessageResponseDto>.ErrorResponse("Debes completar tu perfil antes de continuar."));

        // Act
        var result = await _sut.OfferPriceAsync(Guid.NewGuid(), senderId, 100m, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("completar tu perfil");
        _conversationRepoMock.Verify(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    #endregion

    #region AcceptPriceAsync - Validacion de perfil

    [Fact]
    public async Task AcceptPriceAsync_ConPerfilIncompleto_DebeRetornarError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _profileServiceMock
            .Setup(x => x.ValidateProfileOrError<ConversationResponseDto>(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ApiResponse<ConversationResponseDto>.ErrorResponse("Debes completar tu perfil antes de continuar."));

        // Act
        var result = await _sut.AcceptPriceAsync(Guid.NewGuid(), userId, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("completar tu perfil");
    }

    #endregion

    #region StartConversationAsync - Validaciones de negocio

    [Fact]
    public async Task StartConversationAsync_ProductoNoEncontrado_DebeRetornarError()
    {
        // Arrange
        _productRepoMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _sut.StartConversationAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Producto no encontrado");
    }

    [Fact]
    public async Task StartConversationAsync_ProductoNoNegociable_DebeRetornarError()
    {
        // Arrange
        var product = new Product { IsNegotiable = false };
        _productRepoMock.Setup(x => x.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        // Act
        var result = await _sut.StartConversationAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("no es negociable");
    }

    [Fact]
    public async Task StartConversationAsync_ConversacionYaExiste_DebeRetornarError()
    {
        // Arrange
        var buyerId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var product = new Product { IsNegotiable = true, Company = new Company { OwnerId = Guid.NewGuid() } };

        _productRepoMock.Setup(x => x.GetByIdWithDetailsAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _conversationRepoMock.Setup(x => x.GetActiveByBuyerAndProductAsync(buyerId, productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Conversation());

        // Act
        var result = await _sut.StartConversationAsync(buyerId, productId, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("conversación activa");
    }

    #endregion

    #region SendMessageAsync

    [Fact]
    public async Task SendMessageAsync_ConversacionCerrada_DebeRetornarError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var conversation = new Conversation
        {
            Id = conversationId,
            BuyerId = userId,
            SellerId = Guid.NewGuid(),
            Status = ConversationStatus.Closed
        };

        _conversationRepoMock.Setup(x => x.GetByIdWithDetailsAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        // Act
        var result = await _sut.SendMessageAsync(conversationId, userId, "Hola", CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("cerrada");
    }

    [Fact]
    public async Task SendMessageAsync_SinAcceso_DebeRetornarError()
    {
        // Arrange
        var conversationId = Guid.NewGuid();
        var conversation = new Conversation
        {
            BuyerId = Guid.NewGuid(),
            SellerId = Guid.NewGuid(),
            Status = ConversationStatus.Open
        };

        _conversationRepoMock.Setup(x => x.GetByIdWithDetailsAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        // Act - userId diferente al buyer y seller
        var result = await _sut.SendMessageAsync(conversationId, Guid.NewGuid(), "Hola", CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("No tienes acceso");
    }

    #endregion
}
