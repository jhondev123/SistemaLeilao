using FluentAssertions;
using SistemaLeilao.Core.Domain.Entities;
using Xunit;

namespace SistemaLeilao.UnitTests.Domain.Entities
{
    public class BidderTests
    {
        [Fact]
        public void Constructor_ShouldCreateBidderWithValidData()
        {
            // Arrange
            var perfilName = "Maria";
            var userId = 1L;

            // Act
            var bidder = new Bidder(perfilName, userId);

            // Assert
            bidder.PerfilName.Should().Be(perfilName);
            bidder.Id.Should().Be(userId);
            bidder.WalletBalance.Should().Be(0m);
            bidder.Bids.Should().BeEmpty();
        }

        [Fact]
        public void AddCredits_ShouldIncreaseWalletBalance()
        {
            // Arrange
            var bidder = new Bidder("Maria", 1L);
            var creditsToAdd = 100m;

            // Act
            bidder.AddCredits(creditsToAdd);

            // Assert
            bidder.WalletBalance.Should().Be(creditsToAdd);
        }

        [Fact]
        public void AddCredits_MultipleTimes_ShouldAccumulateBalance()
        {
            // Arrange
            var bidder = new Bidder("Maria", 1L);

            // Act
            bidder.AddCredits(100m);
            bidder.AddCredits(50m);
            bidder.AddCredits(25m);

            // Assert
            bidder.WalletBalance.Should().Be(175m);
        }
    }
}