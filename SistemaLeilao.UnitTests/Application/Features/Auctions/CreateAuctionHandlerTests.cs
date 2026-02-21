using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Resources;
using SistemaLeilao.Tests.Common.Builders;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLeilao.UnitTests.Application.Features.Auctions;

public class CreateAuctionHandlerTests
{
    private readonly Mock<IAuctionRepository> auctionRepositoryMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly Mock<IUserContextService> userContextServiceMock;
    private readonly Mock<ILogger<CreateAuctionHandler>> loggerMock;
    private readonly CreateAuctionHandler handler;

    public CreateAuctionHandlerTests()
    {
        auctionRepositoryMock = new Mock<IAuctionRepository>();
        unitOfWorkMock = new Mock<IUnitOfWork>();
        userContextServiceMock = new Mock<IUserContextService>();
        loggerMock = new Mock<ILogger<CreateAuctionHandler>>();
        handler = new CreateAuctionHandler(
            auctionRepositoryMock.Object,
            unitOfWorkMock.Object,
            userContextServiceMock.Object,
            loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AuctioneerNotFound()
    {
        // Arrange
        CreateAuctionCommand command = new CreateAuctionCommand(
            "Leilão Teste",
            100m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            "Descrição",
            null,
            10m);

        userContextServiceMock
            .Setup(service => service.GetCurrentAuctioneerAsync())
            .ReturnsAsync((Auctioneer?)null);

        // Act
        Result<CreateAuctionResponseDto?> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(new ErrorMessage(nameof(Messages.ErrorAuctioneerNotFound), Messages.ErrorAuctioneerNotFound));
        auctionRepositoryMock.Verify(repository => repository.Add(It.IsAny<Auction>()), Times.Never);
        unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_AuctionIsCreated()
    {
        // Arrange
        Auctioneer auctioneer = new AuctioneerBuilder().Build();
        CreateAuctionCommand command = new CreateAuctionCommand(
            "Leilão Teste",
            100m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            "Descrição",
            null,
            10m);

        userContextServiceMock
            .Setup(service => service.GetCurrentAuctioneerAsync())
            .ReturnsAsync(auctioneer);

        unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        // Act
        Result<CreateAuctionResponseDto?> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result?.Message?.Message. Should().Be(Messages.SuccessAuctionCreated);
        result?.Message?.Code.Should().Be(nameof(Messages.SuccessAuctionCreated));
        result?.Data.Should().NotBeNull();
        auctionRepositoryMock.Verify(repository => repository.Add(
            It.Is<Auction>(auction => auction.Title == command.Title && auction.AuctioneerId == auctioneer.Id)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CommitFails()
    {
        // Arrange
        Auctioneer auctioneer = new AuctioneerBuilder().Build();
        CreateAuctionCommand command = new CreateAuctionCommand(
            "Leilão Teste",
            100m,
            DateTime.UtcNow,
            DateTime.UtcNow.AddDays(1),
            "Descrição",
            null,
            10m);

        userContextServiceMock
            .Setup(service => service.GetCurrentAuctioneerAsync())
            .ReturnsAsync(auctioneer);

        unitOfWorkMock
            .Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        // Act
        Result<CreateAuctionResponseDto?> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(new ErrorMessage(nameof(Messages.ErrorOnCreateAuction), Messages.ErrorOnCreateAuction));
    }
}
