using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SistemaLeilao.Core.Application.Common.Extensions;
using SistemaLeilao.Core.Application.Features.Admin.Commands.CreateUser;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.Infrastructure.Persistence.Repositories;
using SistemaLeilao.IntegrationTests.Fixtures;
using Xunit;

namespace SistemaLeilao.IntegrationTests.Features.Admin
{
    [Collection("DatabaseCollection")]
    public class CreateUserHandlerTests : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMediator _mediator;
        public CreateUserHandlerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;

            var services = new ServiceCollection();

            // Configuração do Identity e DB
            services.AddIdentityCore<User>()
                .AddRoles<Role>()
                .AddEntityFrameworkStores<Infrastructure.Persistence.Contexts.PostgresDbContext>();

            services.AddSingleton(_fixture.Context);
            services.AddScoped<Core.Application.Interfaces.IAuthService, Infrastructure.Services.Auth.AuthService>();
            services.AddScoped<Core.Application.Interfaces.IJwtTokenGeneratorService, Infrastructure.Services.JwtToken.JwtTokenGenerator>();
            services.AddScoped<IAuctioneerRepository, AuctioneerRepository>();
            services.AddScoped<IBidderRepository, BidderRepository>();
            services.AddScoped<IUnitOfWork, Infrastructure.Persistence.Repositories.UnitOfWork>();

            // --- AJUSTE AQUI: Substituindo Moq por NSubstitute ---
            services.AddScoped<ILogger<CreateUserHandler>>(sp => Substitute.For<ILogger<CreateUserHandler>>());

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserHandler).Assembly));

            _serviceProvider = services.BuildServiceProvider();
            _mediator = _serviceProvider.GetRequiredService<IMediator>();
        }

        [Fact]
        public async Task Handle_WithAuctioneerRole_ShouldCreateUserAndAuctioneer()
        {
            // Arrange
            var command = new CreateUserCommand(
                "Leiloeiro Teste",
                "leiloeiro@example.com",
                "P@ssw0rd123!",
                RoleEnum.Auctioneer.GetDescription()
            );

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be("Usuário criado com sucesso!");

            var user = await _fixture.Context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
            user.Should().NotBeNull();

            var auctioneer = await _fixture.Context.Auctioneers.FirstOrDefaultAsync(a => a.Email == command.Email);
            auctioneer.Should().NotBeNull();
            auctioneer!.Name.Should().Be(command.Name);
            auctioneer.Id.Should().Be(user!.Id);
        }

        [Fact]
        public async Task Handle_WithBidderRole_ShouldCreateUserAndBidder()
        {
            // Arrange
            var command = new CreateUserCommand(
                "Arrematante Teste",
                "arrematante@example.com",
                "P@ssw0rd123!",
                RoleEnum.Bidder.GetDescription()
            );

            // Act
            var result = await _mediator.Send(command);

            // Assert
            result.IsSuccess.Should().BeTrue();

            var user = await _fixture.Context.Users.FirstOrDefaultAsync(u => u.Email == command.Email);
            user.Should().NotBeNull();

            var bidder = await _fixture.Context.Bidders.FirstOrDefaultAsync(b => b.Id == user!.Id);
            bidder.Should().NotBeNull();
            bidder!.PerfilName.Should().Be(command.Name);
            bidder.Id.Should().Be(user!.Id);
        }

        [Fact]
        public async Task Handle_WithInvalidEmail_ShouldReturnFailure()
        {
            // Arrange
            var command = new CreateUserCommand(
                "Usuário",
                "invalidemail",
                "P@ssw0rd123!",
                RoleEnum.Bidder.GetDescription()
            );

            // Act
            var result = await _mediator.Send(command);

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
