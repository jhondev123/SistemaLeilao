using FluentAssertions;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Resources;
using SistemaLeilao.Core.Domain.Services.Bid;
using SistemaLeilao.Tests.Common.Builders;
using Xunit;

namespace SistemaLeilao.UnitTests.Domain.Services
{
    public class BidDomainServiceTests
    {
        private readonly BidDomainService _service = new BidDomainService();

        [Fact]
        public void ValidateBid_WhenAuctionIsNull_ShouldReturnFalseWithAuctionNotFoundError()
        {
            // Arrange
            Auction? auction = null;
            Bidder bidder = new BidderBuilder().Build();
            decimal amount = 150m;

            // Act
            var (success, error) = _service.ValidateBid(auction, bidder, amount);

            // Assert
            success.Should().BeFalse();
            error.Code.Should().Be(nameof(Messages.ErrorAuctionNotFound));
            error.Message.Should().Be(Messages.ErrorAuctionNotFound);
        }

        [Fact]
        public void ValidateBid_WhenBidderIsNull_ShouldReturnFalseWithBidderNotFoundError()
        {
            // Arrange
            Auction auction = new AuctionBuilder().WithStatus(AuctionStatus.OPEN).Build();
            Bidder? bidder = null;
            decimal amount = 150m;

            // Act
            var (success, error) = _service.ValidateBid(auction, bidder, amount);

            // Assert
            success.Should().BeFalse();
            error.Code.Should().Be(nameof(Messages.ErrorBidderNotFound));
            error.Message.Should().Be(Messages.ErrorBidderNotFound);
        }

        [Fact]
        public void ValidateBid_WhenAuctioneerTriesToBidOnOwnAuction_ShouldReturnFalse()
        {
            // Arrange
            long sharedId = 5L;
            Auction auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.OPEN)
                .WithAuctioneerId(sharedId)
                .Build();
            Bidder bidder = new BidderBuilder()
                .WithUserId(sharedId)
                .Build();
            decimal amount = 150m;

            // Act
            var (success, error) = _service.ValidateBid(auction, bidder, amount);

            // Assert
            success.Should().BeFalse();
            error.Code.Should().Be(nameof(Messages.ErrorAuctioneerDoNotBidInOwnAuction));
            error.Message.Should().Be(Messages.ErrorAuctioneerDoNotBidInOwnAuction);
        }

        [Fact]
        public void ValidateBid_WhenAuctionIsNotOpen_ShouldReturnFalse()
        {
            // Arrange
            Auction auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.CLOSED)
                .WithAuctioneerId(1L)
                .Build();
            Bidder bidder = new BidderBuilder()
                .WithUserId(2L)
                .Build();
            decimal amount = 150m;

            // Act
            var (success, error) = _service.ValidateBid(auction, bidder, amount);

            // Assert
            success.Should().BeFalse();
            error.Code.Should().Be(nameof(Messages.ErrorAuctionNotOpen));
        }

        [Fact]
        public void ValidateBid_WithValidData_ShouldReturnTrue()
        {
            // Arrange
            Auction auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.OPEN)
                .WithAuctioneerId(1L)
                .WithCurrentPrice(100m)
                .WithMinimumIncrement(10m)
                .Build();
            Bidder bidder = new BidderBuilder()
                .WithUserId(2L)
                .Build();
            decimal amount = 115m;

            // Act
            var (success, error) = _service.ValidateBid(auction, bidder, amount);

            // Assert
            success.Should().BeTrue();
            error.Message.Should().BeEmpty();
        }
    }
}
