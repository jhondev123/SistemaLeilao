using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Resources;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.Infrastructure.Persistence.Repositories;
using SistemaLeilao.IntegrationTests.Fixtures;
using SistemaLeilao.Tests.Common.Builders;
using Xunit;

namespace SistemaLeilao.IntegrationTests.Features.Auctions
{
    [Collection("DatabaseCollection")]
    public class CreateAuctionHandlerTests : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private IMediator _mediator = null!;
        private IUserContextService _userContextService = null!;

        public CreateAuctionHandlerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async ValueTask InitializeAsync()
        {
            await _fixture.ResetAsync();

            _userContextService = Substitute.For<IUserContextService>();

            var services = new ServiceCollection();
            services.AddSingleton(_fixture.Context);
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton(_userContextService);
            services.AddScoped<ILogger<CreateAuctionHandler>>(sp => Substitute.For<ILogger<CreateAuctionHandler>>());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateAuctionHandler).Assembly));

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task Handle_WithValidAuctioneer_ShouldCreateAuction()
        {
            // Arrange
            var auctioneer = await SeedAuctioneerAsync(userId: 1);
            _userContextService.GetCurrentAuctioneerAsync().Returns(auctioneer);

            var command = new CreateAuctionCommand(
                Title: "Leilão de Arte Moderna",
                StartingPrice: 500m,
                StartDate: DateTime.UtcNow.AddHours(1),
                EndDate: DateTime.UtcNow.AddHours(5),
                Description: "Leilão de obras de arte moderna do século XX",
                Image: null,
                MinimumIncrement: 50m);

            // Act
            var result = await _mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Message.Should().Be(new SuccessMessage(nameof(Messages.SuccessAuctionCreated), Messages.SuccessAuctionCreated));
            result.Data.Should().NotBeNull();

            var auction = await _fixture.Context.Auctions
                .FirstOrDefaultAsync(a => a.Title == command.Title, TestContext.Current.CancellationToken);
            auction.Should().NotBeNull();
            auction!.AuctioneerId.Should().Be(auctioneer.Id);
            auction.Status.Should().Be(AuctionStatus.AWAITING_START);
            auction.StartingPrice.Should().Be(command.StartingPrice);
        }

        [Fact]
        public async Task Handle_WhenAuctioneerNotFound_ShouldReturnFailure()
        {
            // Arrange
            _userContextService.GetCurrentAuctioneerAsync().Returns((Auctioneer?)null);

            var command = new CreateAuctionCommand(
                Title: "Leilão sem leiloeiro",
                StartingPrice: 100m,
                StartDate: DateTime.UtcNow.AddHours(1),
                EndDate: DateTime.UtcNow.AddHours(3),
                Description: null,
                Image: null,
                MinimumIncrement: 10m);

            // Act
            var result = await _mediator.Send(command, TestContext.Current.CancellationToken);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(new ErrorMessage(nameof(Messages.ErrorAuctioneerNotFound), Messages.ErrorAuctioneerNotFound));

            var anyAuction = await _fixture.Context.Auctions.AnyAsync(TestContext.Current.CancellationToken);
            anyAuction.Should().BeFalse();
        }

        private async Task<Auctioneer> SeedAuctioneerAsync(long userId)
        {
            var user = CreateIdentityUser(userId, $"auctioneer{userId}@test.com");
            _fixture.Context.Users.Add(user);

            var auctioneer = new AuctioneerBuilder()
                .WithUserId(userId)
                .WithEmail($"auctioneer{userId}@test.com")
                .Build();
            _fixture.Context.Auctioneers.Add(auctioneer);

            await _fixture.Context.SaveChangesAsync(TestContext.Current.CancellationToken);
            return auctioneer;
        }

        private static User CreateIdentityUser(long id, string email) => new User
        {
            Id = id,
            UserName = email,
            NormalizedUserName = email.ToUpperInvariant(),
            Email = email,
            NormalizedEmail = email.ToUpperInvariant(),
            SecurityStamp = Guid.NewGuid().ToString()
        };
    }
}
