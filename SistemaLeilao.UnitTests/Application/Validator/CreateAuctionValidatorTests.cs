using FluentAssertions;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using Xunit;

namespace SistemaLeilao.UnitTests.Application.Validator
{
    public class CreateAuctionValidatorTests
    {
        private readonly CreateAuctionValidator _validator = new CreateAuctionValidator();

        private CreateAuctionCommand ValidCommand() => new CreateAuctionCommand(
            Title: "Leilão de Arte Moderna",
            StartingPrice: 100m,
            StartDate: DateTime.UtcNow.AddMinutes(5),
            EndDate: DateTime.UtcNow.AddHours(3),
            Description: "Descrição detalhada do leilão de arte",
            Image: null,
            MinimumIncrement: 10m);

        [Fact]
        public void Validate_WhenTitleIsEmpty_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Title = "" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Title));
        }

        [Fact]
        public void Validate_WhenTitleIsTooShort_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Title = "Abc" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Title));
        }

        [Fact]
        public void Validate_WhenTitleIsTooLong_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Title = new string('A', 101) };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Title));
        }

        [Fact]
        public void Validate_WhenStartingPriceIsZero_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { StartingPrice = 0m };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.StartingPrice));
        }

        [Fact]
        public void Validate_WhenStartingPriceIsNegative_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { StartingPrice = -50m };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.StartingPrice));
        }

        [Fact]
        public void Validate_WhenMinimumIncrementIsZero_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { MinimumIncrement = 0m };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.MinimumIncrement));
        }

        [Fact]
        public void Validate_WhenMinimumIncrementIsGreaterThanStartingPrice_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { StartingPrice = 50m, MinimumIncrement = 100m };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.MinimumIncrement));
        }

        [Fact]
        public void Validate_WhenEndDateIsBeforeStartDate_ShouldFail()
        {
            // Arrange
            var start = DateTime.UtcNow.AddHours(2);
            var command = ValidCommand() with { StartDate = start, EndDate = start.AddMinutes(-1) };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.EndDate));
        }

        [Fact]
        public void Validate_WhenDurationIsLessThanOneHour_ShouldFail()
        {
            // Arrange
            var start = DateTime.UtcNow.AddMinutes(5);
            var command = ValidCommand() with { StartDate = start, EndDate = start.AddMinutes(30) };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.EndDate));
        }

        [Fact]
        public void Validate_WhenDescriptionIsTooShort_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Description = "Curta" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Description));
        }

        [Fact]
        public void Validate_WhenImageExceedsMaxSize_ShouldFail()
        {
            // Arrange
            var oversizedImage = new byte[2 * 1024 * 1024 + 1];
            var command = ValidCommand() with { Image = oversizedImage };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Image));
        }

        [Fact]
        public void Validate_WhenAllDataIsValid_ShouldPass()
        {
            // Arrange
            var command = ValidCommand();

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
            result.Errors.Should().BeEmpty();
        }
    }
}
