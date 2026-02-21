using FluentAssertions;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Resources;
using SistemaLeilao.Tests.Common.Builders;
using System;
using System.Globalization;
using Xunit;

namespace SistemaLeilao.UnitTests.Domain.Entities
{
    public class AuctionTests
    {
        [Fact]
        public void Constructor_ShouldCreateAuctionWithValidData()
        {
            // Arrange
            var title = "Leilão de Arte";
            var auctioneerId = 1L;
            var startingPrice = 100m;
            var currentPrice = 100m;
            var startDate = DateTime.UtcNow.AddDays(1);
            var endDate = DateTime.UtcNow.AddDays(2);
            var description = "Descrição do leilão";
            byte[]? image = null;
            var minimumIncrement = 10m;
            var status = AuctionStatus.OPEN;

            // Act
            var auction = new Auction(title, auctioneerId, startingPrice, currentPrice, startDate, endDate, description, image, minimumIncrement, status);

            // Assert
            auction.Title.Should().Be(title);
            auction.AuctioneerId.Should().Be(auctioneerId);
            auction.StartingPrice.Should().Be(startingPrice);
            auction.CurrentPrice.Should().Be(currentPrice);
            auction.StartDate.Should().Be(startDate);
            auction.EndDate.Should().Be(endDate);
            auction.Description.Should().Be(description);
            auction.MinimumIncrement.Should().Be(minimumIncrement);
            auction.Status.Should().Be(status);
        }

        [Fact]
        public void CanPlaceBid_WhenAuctionIsNotOpen_ShouldReturnFalse()
        {
            // Arrange
            var auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.CLOSED)
                .Build();

            // Act
            var (success, errorMessage) = auction.CanPlaceBid(150m);

            // Assert
            success.Should().BeFalse();
            errorMessage.Message.Should().Be(Messages.ErrorAuctionNotOpen);
            errorMessage.Code.Should().Be(nameof(Messages.ErrorAuctionNotOpen));
        }

        [Fact]
        public void CanPlaceBid_WhenAuctionHasEnded_ShouldReturnFalse()
        {
            // Arrange
            var auction = new AuctionBuilder()
            .WithStatus(AuctionStatus.OPEN)
            .WithStartDate(DateTime.UtcNow.AddDays(-2))
            .WithEndDate(DateTime.UtcNow.AddDays(-1))
            .Build();

            // Act
            var (success, errorMessage) = auction.CanPlaceBid(150m);

            // Assert
            success.Should().BeFalse();
            errorMessage.Message.Should().Be(Messages.ErrorAuctionEnded);
            errorMessage.Code.Should().Be(nameof(Messages.ErrorAuctionEnded));
        }

        [Fact]
        public void CanPlaceBid_WhenBidAmountIsLessThanMinimumIncrement_ShouldReturnFalse()
        {
            // Arrange
            decimal minimumIncrement = 10m;
            decimal insufficientBidAmount = minimumIncrement - 1m;

            var auction = new AuctionBuilder()
             .WithStatus(AuctionStatus.OPEN)
             .WithMinimumIncrement(minimumIncrement)
             .Build();

            // Act
            var (success, errorMessage) = auction.CanPlaceBid(insufficientBidAmount);

            // Assert
            success.Should().BeFalse();
            string expectedValue = minimumIncrement.ToString("C", CultureInfo.CurrentUICulture);
            errorMessage.Message.Should().Contain(expectedValue);
            errorMessage.Code.Should().Be(nameof(Messages.ErrorBidTooLow));
        }

        [Fact]
        public void CanPlaceBid_WhenBidIsValid_ShouldReturnTrue()
        {
            // Arrange
            decimal minimumIncrement = 10m;

            var auction = new AuctionBuilder()
             .WithStatus(AuctionStatus.OPEN)
             .WithMinimumIncrement(minimumIncrement)
             .Build();

            decimal validBidAmount = minimumIncrement + auction.CurrentPrice;

            // Act
            var (success, errorMessage) = auction.CanPlaceBid(validBidAmount);

            // Assert
            success.Should().BeTrue();
            errorMessage.Message.Should().BeEmpty();
            errorMessage.Code.Should().BeEmpty();
        }

        [Fact]
        public void ApplyNewBid_WhenBidIsValid_ShouldUpdateCurrentPriceAndWinner()
        {
            // Arrange
            var auction = new AuctionBuilder()
             .WithStatus(AuctionStatus.OPEN)
             .Build();
            var newPrice = 150m;
            var bidderId = 5L;

            // Act
            var (success, errorMessage) = auction.ApplyNewBid(newPrice, bidderId);

            // Assert
            success.Should().BeTrue();
            errorMessage.Message.Should().BeEmpty();
            errorMessage.Code.Should().BeEmpty();
            auction.CurrentPrice.Should().Be(newPrice);
            auction.BidderWinnerId.Should().Be(bidderId);
        }

        [Fact]
        public void ApplyNewBid_WhenBidIsInvalid_ShouldNotUpdateAuction()
        {
            // Arrange
            var auction = new AuctionBuilder()
             .WithStatus(AuctionStatus.CLOSED)
             .Build();

            var originalPrice = auction.CurrentPrice;

            // Act
            var (success, errorMessage) = auction.ApplyNewBid(150m, 5L);

            // Assert
            success.Should().BeFalse();
            errorMessage.Message.Should().Be(Messages.ErrorAuctionNotOpen);
            errorMessage.Code.Should().Be(nameof(Messages.ErrorAuctionNotOpen));
            auction.CurrentPrice.Should().Be(originalPrice);
            auction.BidderWinnerId.Should().BeNull();
        }
    }
}
