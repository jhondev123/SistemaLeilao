using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid;
using SistemaLeilao.Core.Application.Features.Bid.Events;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Resources;
using SistemaLeilao.Core.Domain.Services.Bid;
using SistemaLeilao.Tests.Common.Builders;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLeilao.UnitTests.Application.Features.Bid;

public class CreateBidHandlerTests
{
    private readonly Mock<IPublishEndpoint> publishEndpointMock;
    private readonly Mock<IUserContextService> userContextServiceMock;
    private readonly Mock<ILogger<CreateBidHandler>> loggerMock;
    private readonly Mock<IAuctionRepository> auctionRepositoryMock;
    private readonly BidDomainService bidDomainService;
    private readonly CreateBidHandler handler;

    public CreateBidHandlerTests()
    {
        publishEndpointMock = new Mock<IPublishEndpoint>();
        userContextServiceMock = new Mock<IUserContextService>();
        loggerMock = new Mock<ILogger<CreateBidHandler>>();
        auctionRepositoryMock = new Mock<IAuctionRepository>();
        bidDomainService = new BidDomainService();
        handler = new CreateBidHandler(
            publishEndpointMock.Object,
            userContextServiceMock.Object,
            loggerMock.Object,
            auctionRepositoryMock.Object,
            bidDomainService);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BidIsValid()
    {
        // Arrange
        Guid auctionId = Guid.NewGuid();
        Guid bidderExternalId = Guid.NewGuid();
        Guid userExternalId = Guid.NewGuid();

        Auction auction = new AuctionBuilder()
            .WithStatus(AuctionStatus.OPEN)
            .WithEndDate(DateTime.UtcNow.AddHours(1))
            .WithAuctioneerId(10L)
            .WithExternalId(auctionId)
            .Build();

        Bidder bidder = new BidderBuilder()
            .WithUserId(20L)
            .WithExternalId(bidderExternalId)
            .Build();

        CreateBidCommand command = new CreateBidCommand(auctionId, 120m);

        userContextServiceMock.Setup(service => service.GetUserExternalId()).Returns(userExternalId);
        userContextServiceMock.Setup(service => service.GetCurrentBidderAsync()).ReturnsAsync(bidder);
        auctionRepositoryMock.Setup(repository => repository.GetByExternalIdAsync(auctionId)).ReturnsAsync(auction);
        publishEndpointMock.Setup(endpoint => endpoint.Publish(It.IsAny<BidPlacedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result?.Message?.Message.Should().Be(Messages.SuccessBidSentToProcessing);
        publishEndpointMock.Verify(endpoint => endpoint.Publish(
            It.Is<BidPlacedEvent>(evt =>
                evt.AuctionId == auctionId &&
                evt.BidderId == bidderExternalId &&
                evt.UserExternalId == userExternalId &&
                evt.Amount == command.Amount),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BidderIsNull()
    {
        // Arrange
        Guid auctionId = Guid.NewGuid();
        Auction auction = new AuctionBuilder().WithExternalId(auctionId).Build();
        CreateBidCommand command = new CreateBidCommand(auctionId, 120m);

        userContextServiceMock.Setup(service => service.GetUserExternalId()).Returns(Guid.NewGuid());
        userContextServiceMock.Setup(service => service.GetCurrentBidderAsync()).ReturnsAsync((Bidder?)null);
        auctionRepositoryMock.Setup(repository => repository.GetByExternalIdAsync(auctionId)).ReturnsAsync(auction);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(new ErrorMessage(nameof(Messages.ErrorUserNotAuthenticated), Messages.ErrorUserNotAuthenticated));
        publishEndpointMock.Verify(endpoint => endpoint.Publish(It.IsAny<BidPlacedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BidIsInvalid()
    {
        // Arrange
        Guid auctionId = Guid.NewGuid();
        Auction auction = new AuctionBuilder()
            .WithStatus(AuctionStatus.CLOSED)
            .WithExternalId(auctionId)
            .Build();

        Bidder bidder = new BidderBuilder()
            .WithUserId(20L)
            .Build();

        CreateBidCommand command = new CreateBidCommand(auctionId, 120m);

        userContextServiceMock.Setup(service => service.GetUserExternalId()).Returns(Guid.NewGuid());
        userContextServiceMock.Setup(service => service.GetCurrentBidderAsync()).ReturnsAsync(bidder);
        auctionRepositoryMock.Setup(repository => repository.GetByExternalIdAsync(auctionId)).ReturnsAsync(auction);

        // Act
        Result result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(new ErrorMessage(nameof(Messages.ErrorAuctionNotOpen), Messages.ErrorAuctionNotOpen));
        publishEndpointMock.Verify(endpoint => endpoint.Publish(It.IsAny<BidPlacedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
