using FluentAssertions;
using Moq;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Auth.Commands.LoginUser;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Resources;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLeilao.UnitTests.Application.Features.Auth;

public class LoginUserHandlerTests
{
    private readonly Mock<IAuthService> authServiceMock;
    private readonly LoginUserHandler handler;

    public LoginUserHandlerTests()
    {
        authServiceMock = new Mock<IAuthService>();
        handler = new LoginUserHandler(authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_CredentialsAreValid()
    {
        // Arrange
        LoginUserCommand command = new LoginUserCommand("user@example.com", "P@ssw0rd123!");

        authServiceMock
            .Setup(service => service.LoginAsync(command.Email, command.Password))
            .ReturnsAsync((true, "token-value"));

        // Act
        Result<string> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be("token-value");
        result?.Message?.Message.Should().Be(Messages.SuccessLoginSuccessful);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CredentialsAreInvalid()
    {
        // Arrange
        LoginUserCommand command = new LoginUserCommand("user@example.com", "invalid");

        authServiceMock
            .Setup(service => service.LoginAsync(command.Email, command.Password))
            .ReturnsAsync((false, (string?)null));

        // Act
        Result<string> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Errors.Should().Contain(new ErrorMessage(nameof(Messages.ErrorInvalidEmailOrPassword), Messages.ErrorInvalidEmailOrPassword));
    }
}
