using FluentAssertions;
using SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid;
using Xunit;

namespace SistemaLeilao.UnitTests.Application.Validator
{
    public class CreateBidValidatorTests
    {
        private readonly CreateBidValidator _validator = new CreateBidValidator();

        [Fact]
        public void Validate_WhenAuctionIdIsEmpty_ShouldFail()
        {
            // Arrange
            var command = new CreateBidCommand(Guid.Empty, 150m);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.AuctionId));
        }

        [Fact]
        public void Validate_WhenAmountIsZero_ShouldFail()
        {
            // Arrange
            var command = new CreateBidCommand(Guid.NewGuid(), 0m);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Amount));
        }

        [Fact]
        public void Validate_WhenAmountIsNegative_ShouldFail()
        {
            // Arrange
            var command = new CreateBidCommand(Guid.NewGuid(), -50m);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Amount));
        }

        [Fact]
        public void Validate_WhenAmountHasMoreThanTwoDecimalPlaces_ShouldFail()
        {
            // Arrange
            var command = new CreateBidCommand(Guid.NewGuid(), 150.123m);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Amount));
        }

        [Fact]
        public void Validate_WhenAmountIsValidWithTwoDecimalPlaces_ShouldPass()
        {
            // Arrange
            var command = new CreateBidCommand(Guid.NewGuid(), 150.50m);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }

        [Fact]
        public void Validate_WhenAllDataIsValid_ShouldPass()
        {
            // Arrange
            var command = new CreateBidCommand(Guid.NewGuid(), 200m);

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}
