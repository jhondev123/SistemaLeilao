using FluentAssertions;
using SistemaLeilao.Core.Application.Features.Admin.Commands.CreateUser;
using SistemaLeilao.Core.Domain.Enums;
using Xunit;

namespace SistemaLeilao.UnitTests.Application.Validator
{
    public class CreateUserValidatorTests
    {
        private readonly CreateUserValidator _validator = new CreateUserValidator();

        private CreateUserCommand ValidCommand() => new CreateUserCommand(
            Name: "João Silva",
            Email: "joao@example.com",
            Password: "P@ssw0rd123!",
            Role: RoleEnum.Bidder.ToString());

        [Fact]
        public void Validate_WhenNameIsEmpty_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Name = "" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public void Validate_WhenNameIsTooShort_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Name = "Jo" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public void Validate_WhenNameIsTooLong_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Name = new string('A', 101) };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Name));
        }

        [Fact]
        public void Validate_WhenEmailIsEmpty_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Email = "" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email));
        }

        [Fact]
        public void Validate_WhenEmailIsInvalidFormat_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Email = "email-invalido" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Email));
        }

        [Fact]
        public void Validate_WhenPasswordIsEmpty_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Password = "" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }

        [Fact]
        public void Validate_WhenPasswordIsTooShort_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Password = "Ab1!" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }

        [Fact]
        public void Validate_WhenPasswordLacksUppercase_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Password = "p@ssw0rd123!" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }

        [Fact]
        public void Validate_WhenPasswordLacksNumber_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Password = "P@ssword!" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }

        [Fact]
        public void Validate_WhenPasswordLacksSpecialChar_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Password = "Passw0rd123" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Password));
        }

        [Fact]
        public void Validate_WhenRoleIsEmpty_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Role = "" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Role));
        }

        [Fact]
        public void Validate_WhenRoleIsInvalid_ShouldFail()
        {
            // Arrange
            var command = ValidCommand() with { Role = "CargoInexistente" };

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == nameof(command.Role));
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
