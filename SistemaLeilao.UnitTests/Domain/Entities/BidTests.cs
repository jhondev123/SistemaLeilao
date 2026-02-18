using FluentAssertions;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using Xunit;

namespace SistemaLeilao.UnitTests.Domain.Entities
{
    public class BidTests
    {
        [Fact]
        public void Constructor_ShouldCreateBidWithValidData()
        {
            // Arrange
            var amount = 150m;
            var auction = CreateValidAuction();
            var bidder = new Bidder("Maria", 1L);
            var beforeCreation = DateTime.UtcNow;

            // Act
            var bid = new Bid(amount, auction, bidder);
            var afterCreation = DateTime.UtcNow;

            // Assert
            bid.Amount.Should().Be(amount);
            bid.Auction.Should().Be(auction);
            bid.Bidder.Should().Be(bidder);
            bid.BidDate.Should().BeOnOrAfter(beforeCreation);
            bid.BidDate.Should().BeOnOrBefore(afterCreation);
        }

        private Auction CreateValidAuction()
        {
            return new Auction(
                "Leilão Teste",
                1L,
                100m,
                100m,
                DateTime.UtcNow.AddHours(-1),
                DateTime.UtcNow.AddDays(1),
                "Descrição",
                null,
                10m,
                AuctionStatus.OPEN
            );
        }
    }
}