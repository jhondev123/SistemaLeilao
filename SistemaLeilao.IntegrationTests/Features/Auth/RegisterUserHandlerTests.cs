using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SistemaLeilao.Core.Application.Features.Auth.Commands.RegisterUser;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.IntegrationTests.Fixtures;
using Xunit;

namespace SistemaLeilao.IntegrationTests.Features.Auth
{
    [Collection("DatabaseCollection")]
    public class RegisterUserHandlerTests : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMediator _mediator;

        public RegisterUserHandlerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;

            var services = new ServiceCollection();
            services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<Infrastructure.Persistence.Contexts.PostgresDbContext>();

            services.AddSingleton(_fixture.Context);
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(RegisterUserHandler).Assembly));
            services.AddScoped<IAuthService, Infrastructure.Services.Auth.AuthService>();
            services.AddScoped<IJwtTokenGeneratorService, Infrastructure.Services.JwtToken.JwtTokenGenerator>();

            _serviceProvider = services.BuildServiceProvider();
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Handle_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var command = new RegisterUserCommand(
                "João Silva",
                "joao@example.com",
                "P@ssw0rd123!",
                false
            );

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Usuário criado com sucesso!");

            var user = await _fixture.Context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
            user.Should().NotBeNull();
            user!.Email.Should().Be(command.Email);
        }

        [Fact]
        public async Task Handle_WithAuctioneerFlag_ShouldCreateUserWithAuctioneerRole()
        {
            // Arrange
            var command = new RegisterUserCommand(
                "Maria Santos",
                "maria@example.com",
                "P@ssw0rd123!",
                true
            );

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var user = await _fixture.Context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
            user.Should().NotBeNull();

            var userManager = _serviceProvider.GetRequiredService<UserManager<User>>();
            var roles = await userManager.GetRolesAsync(user!);
            roles.Should().Contain("Bidder");
            roles.Should().Contain("Auctioneer");
        }

        [Fact]
        public async Task Handle_WithDuplicateEmail_ShouldReturnFailure()
        {
            // Arrange
            var email = "duplicate@example.com";
            var firstCommand = new RegisterUserCommand("Usuário 1", email, "P@ssw0rd123!", false);
            await _mediator.Send(firstCommand);

            var secondCommand = new RegisterUserCommand("Usuário 2", email, "P@ssw0rd123!", false);

            // Act
            var result = await _mediator.Send(secondCommand);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().NotBeEmpty();
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