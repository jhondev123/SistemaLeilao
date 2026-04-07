using FluentAssertions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SistemaLeilao.Core.Application.Features.Bid.Consumers;
using SistemaLeilao.Core.Application.Features.Bid.Events;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Services.Bid;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.Infrastructure.Persistence.Repositories;
using SistemaLeilao.IntegrationTests.Fixtures;
using SistemaLeilao.Tests.Common.Builders;
using Xunit;

namespace SistemaLeilao.IntegrationTests.Features.Bid
{
    [Collection("DatabaseCollection")]
    public class BidPlacedConsumerTests : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private IAuctionNotificationService _notificationService = null!;
        private IAuctionRepository _auctionRepository = null!;
        private IBidderRepository _bidderRepository = null!;
        private IUnitOfWork _unitOfWork = null!;
        private BidPlacedConsumer _consumer = null!;

        public BidPlacedConsumerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async ValueTask InitializeAsync()
        {
            await _fixture.ResetAsync();

            _notificationService = Substitute.For<IAuctionNotificationService>();
            _notificationService.NotifyNewBid(Arg.Any<Guid>(), Arg.Any<decimal>(), Arg.Any<Guid>()).Returns(Task.CompletedTask);
            _notificationService.NotifyBidRejected(Arg.Any<Guid>(), Arg.Any<ErrorMessage>()).Returns(Task.CompletedTask);

            _auctionRepository = new AuctionRepository(_fixture.Context);
            _bidderRepository = new BidderRepository(_fixture.Context);
            _unitOfWork = new UnitOfWork(_fixture.Context);

            _consumer = new BidPlacedConsumer(
                _notificationService,
                _auctionRepository,
                _bidderRepository,
                _unitOfWork,
                new BidDomainService(),
                Substitute.For<ILogger<BidPlacedConsumer>>());
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task Consume_WhenBidIsValid_ShouldPersistBidAndUpdateAuctionPrice()
        {
            // Arrange
            var auctionExternalId = Guid.NewGuid();
            var bidderExternalId = Guid.NewGuid();
            var userExternalId = Guid.NewGuid();
            const long auctioneerId = 1L;
            const long bidderId = 2L;

            await SeedUsersAsync(new[] { (auctioneerId, "auctioneer@test.com"), (bidderId, "bidder@test.com") });

            var auctioneer = new AuctioneerBuilder().WithUserId(auctioneerId).WithEmail("auctioneer@test.com").Build();
            _fixture.Context.Auctioneers.Add(auctioneer);

            var bidder = new BidderBuilder().WithUserId(bidderId).WithExternalId(bidderExternalId).Build();
            bidder.Name = "Bidder de Teste";
            bidder.Email = "bidder@test.com";
            _fixture.Context.Bidders.Add(bidder);

            var auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.OPEN)
                .WithAuctioneerId(auctioneerId)
                .WithCurrentPrice(100m)
                .WithMinimumIncrement(10m)
                .WithExternalId(auctionExternalId)
                .Build();
            _fixture.Context.Auctions.Add(auction);

            await _fixture.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            decimal bidAmount = 120m;
            var @event = new BidPlacedEvent(auctionExternalId, bidderExternalId, userExternalId, bidAmount);
            var context = CreateConsumeContext(@event);

            // Act
            await _consumer.Consume(context);

            // Assert
            _fixture.Context.ChangeTracker.Clear();

            var updatedAuction = await _fixture.Context.Auctions
                .Include(a => a.Bids)
                .FirstAsync(a => a.Id == auction.Id, TestContext.Current.CancellationToken);

            updatedAuction.CurrentPrice.Should().Be(bidAmount);
            updatedAuction.BidderWinnerId.Should().Be(bidderId);
            updatedAuction.Bids.Should().HaveCount(1);
            updatedAuction.Bids[0].Amount.Should().Be(bidAmount);

            await _notificationService.Received(1)
                .NotifyNewBid(auctionExternalId, bidAmount, userExternalId);
        }

        [Fact]
        public async Task Consume_WhenAuctionNotFound_ShouldNotifyRejectionAndNotPersist()
        {
            // Arrange
            var userExternalId = Guid.NewGuid();
            var @event = new BidPlacedEvent(Guid.NewGuid(), Guid.NewGuid(), userExternalId, 150m);
            var context = CreateConsumeContext(@event);

            // Act
            await _consumer.Consume(context);

            // Assert
            await _notificationService.Received(1).NotifyBidRejected(userExternalId, Arg.Any<ErrorMessage>());
            await _notificationService.DidNotReceive().NotifyNewBid(Arg.Any<Guid>(), Arg.Any<decimal>(), Arg.Any<Guid>());

            var anyBid = await _fixture.Context.Bids.AnyAsync(TestContext.Current.CancellationToken);
            anyBid.Should().BeFalse();
        }

        [Fact]
        public async Task Consume_WhenAuctioneerTriesToBidOnOwnAuction_ShouldNotifyRejectionAndNotPersist()
        {
            // Arrange
            const long sharedId = 5L;
            var auctionExternalId = Guid.NewGuid();
            var bidderExternalId = Guid.NewGuid();
            var userExternalId = Guid.NewGuid();

            await SeedUsersAsync(new[] { (sharedId, "shared@test.com") });

            var auctioneer = new AuctioneerBuilder().WithUserId(sharedId).WithEmail("shared@test.com").Build();
            _fixture.Context.Auctioneers.Add(auctioneer);

            var bidder = new BidderBuilder().WithUserId(sharedId).WithExternalId(bidderExternalId).Build();
            bidder.Name = "Shared User";
            bidder.Email = "shared@test.com";
            _fixture.Context.Bidders.Add(bidder);

            var auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.OPEN)
                .WithAuctioneerId(sharedId)
                .WithExternalId(auctionExternalId)
                .Build();
            _fixture.Context.Auctions.Add(auction);
            await _fixture.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var @event = new BidPlacedEvent(auctionExternalId, bidderExternalId, userExternalId, 150m);
            var context = CreateConsumeContext(@event);

            // Act
            await _consumer.Consume(context);

            // Assert
            await _notificationService.Received(1).NotifyBidRejected(userExternalId, Arg.Any<ErrorMessage>());
            await _notificationService.DidNotReceive().NotifyNewBid(Arg.Any<Guid>(), Arg.Any<decimal>(), Arg.Any<Guid>());
        }

        [Fact]
        public async Task Consume_WhenAuctionIsClosed_ShouldNotifyRejectionAndNotPersist()
        {
            // Arrange
            var auctionExternalId = Guid.NewGuid();
            var bidderExternalId = Guid.NewGuid();
            var userExternalId = Guid.NewGuid();
            const long auctioneerId = 1L;
            const long bidderId = 2L;

            await SeedUsersAsync(new[] { (auctioneerId, "auctioneer@test.com"), (bidderId, "bidder@test.com") });

            var auctioneer = new AuctioneerBuilder().WithUserId(auctioneerId).WithEmail("auctioneer@test.com").Build();
            _fixture.Context.Auctioneers.Add(auctioneer);

            var bidder = new BidderBuilder().WithUserId(bidderId).WithExternalId(bidderExternalId).Build();
            bidder.Name = "Bidder de Teste";
            bidder.Email = "bidder@test.com";
            _fixture.Context.Bidders.Add(bidder);

            var auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.CLOSED)
                .WithAuctioneerId(auctioneerId)
                .WithExternalId(auctionExternalId)
                .Build();
            _fixture.Context.Auctions.Add(auction);
            await _fixture.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            var @event = new BidPlacedEvent(auctionExternalId, bidderExternalId, userExternalId, 150m);
            var context = CreateConsumeContext(@event);

            // Act
            await _consumer.Consume(context);

            // Assert
            await _notificationService.Received(1).NotifyBidRejected(userExternalId, Arg.Any<ErrorMessage>());
            await _notificationService.DidNotReceive().NotifyNewBid(Arg.Any<Guid>(), Arg.Any<decimal>(), Arg.Any<Guid>());
        }

        private async Task SeedUsersAsync(IEnumerable<(long Id, string Email)> users)
        {
            foreach (var (id, email) in users)
            {
                _fixture.Context.Users.Add(new User
                {
                    Id = id,
                    UserName = email,
                    NormalizedUserName = email.ToUpperInvariant(),
                    Email = email,
                    NormalizedEmail = email.ToUpperInvariant(),
                    SecurityStamp = Guid.NewGuid().ToString()
                });
            }
            await _fixture.Context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        private static ConsumeContext<BidPlacedEvent> CreateConsumeContext(BidPlacedEvent @event)
        {
            var context = Substitute.For<ConsumeContext<BidPlacedEvent>>();
            context.Message.Returns(@event);
            context.CancellationToken.Returns(CancellationToken.None);
            return context;
        }
    }
}
