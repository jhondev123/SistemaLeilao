using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SistemaLeilao.Core.Application.Features.Auth.Commands.LoginUser;
using SistemaLeilao.Core.Application.Features.Auth.Commands.RegisterUser;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Resources;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.IntegrationTests.Fixtures;
using Xunit;

namespace SistemaLeilao.IntegrationTests.Features.Auth
{
    [Collection("DatabaseCollection")]
    public class LoginUserHandlerTests : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMediator _mediator;

        public LoginUserHandlerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;

            var services = new ServiceCollection();
            services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<Infrastructure.Persistence.Contexts.PostgresDbContext>()
                .AddSignInManager();

            services.AddSingleton(_fixture.Context);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginUserHandler).Assembly));
            services.AddScoped<Core.Application.Interfaces.IAuthService, Infrastructure.Services.Auth.AuthService>();
            services.AddScoped<Core.Application.Interfaces.IJwtTokenGeneratorService, Infrastructure.Services.JwtToken.JwtTokenGenerator>();

            _serviceProvider = services.BuildServiceProvider();
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Handle_WithValidCredentials_ShouldReturnToken()
        {
            // Arrange
            var email = "login@example.com";
            var password = "P@ssw0rd123!";

            var registerCommand = new RegisterUserCommand("Usuário Teste", email, password, false);
            await _mediator.Send(registerCommand, TestContext.Current.CancellationToken);

            var loginCommand = new LoginUserCommand(email, password);

            // Act
            var result = await _mediator.Send(loginCommand, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNullOrEmpty();
            result.Message.Should().Be(new SuccessMessage(nameof(Messages.SuccessLoginSuccessful), Messages.SuccessLoginSuccessful));
        }

        [Fact]
        public async Task Handle_WithInvalidEmail_ShouldReturnFailure()
        {
            // Arrange
            var command = new LoginUserCommand("naoexiste@example.com", "P@ssw0rd123!");

            // Act
            var result = await _mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(new ErrorMessage(nameof(Messages.ErrorInvalidEmailOrPassword), Messages.ErrorInvalidEmailOrPassword));
        }

        [Fact]
        public async Task Handle_WithInvalidPassword_ShouldReturnFailure()
        {
            // Arrange
            var email = "user@example.com";
            var correctPassword = "P@ssw0rd123!";
            var wrongPassword = "WrongP@ss123!";

            var registerCommand = new RegisterUserCommand("Usuário", email, correctPassword, false);
            await _mediator.Send(registerCommand, TestContext.Current.CancellationToken);

            var loginCommand = new LoginUserCommand(email, wrongPassword);

            // Act
            var result = await _mediator.Send(loginCommand, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(new ErrorMessage(nameof(Messages.ErrorInvalidEmailOrPassword), Messages.ErrorInvalidEmailOrPassword));
        }

        internal async Task InitializeAsync()
        {
            await _fixture.InitializeAsync();
        }

        internal async Task DisposeAsync()
        {
            await _fixture.ResetAsync();
        }

        ValueTask IAsyncLifetime.InitializeAsync()
        {
            throw new NotImplementedException();
        }

        ValueTask IAsyncDisposable.DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
