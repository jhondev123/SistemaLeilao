using FluentAssertions;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Tests.Common.Builders;
using Xunit;

namespace SistemaLeilao.UnitTests.Domain.Entities
{
    public class AuctioneerTests
    {
        [Fact]
        public void Constructor_ShouldCreateAuctioneerWithValidData()
        {
            // Arrange
            var name = "João Silva";
            var email = "joao@example.com";
            var userId = 1L;

            // Act
            var auctioneer = new Auctioneer(name, email, userId);

            // Assert
            auctioneer.Name.Should().Be(name);
            auctioneer.Email.Should().Be(email);
            auctioneer.Id.Should().Be(userId);
            auctioneer.Rating.Should().Be(5.0);
            auctioneer.Auctions.Should().BeEmpty();
        }

        [Fact]
        public void DefaultConstructor_ShouldCreateAuctioneerWithDefaultValues()
        {
            // Arrange
            double rating = 5;
            string? bio = null;
            List<Auction> auctions = new List<Auction>();

            // Act

            var auctioneer = new AuctioneerBuilder()
                .WithRating(rating)
                .WithBio(bio)
                .WithAuctions(auctions)
                .Build();

            // Assert
            auctioneer.Rating.Should().Be(rating);
            auctioneer.Auctions.Should().NotBeNull();
            auctioneer.Auctions.Should().BeEquivalentTo(auctions);
            auctioneer.Bio.Should().Be(bio);
        }
    }
}