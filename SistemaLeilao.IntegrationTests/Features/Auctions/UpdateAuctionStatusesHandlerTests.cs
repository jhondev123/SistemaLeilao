using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.UpdateAuctionStatuses;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Indentity;
using SistemaLeilao.Infrastructure.Persistence.Repositories;
using SistemaLeilao.IntegrationTests.Fixtures;
using SistemaLeilao.Tests.Common.Builders;
using Xunit;

namespace SistemaLeilao.IntegrationTests.Features.Auctions
{
    [Collection("DatabaseCollection")]
    public class UpdateAuctionStatusesHandlerTests : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;
        private IMediator _mediator = null!;
        private IAuctionNotificationService _notificationService = null!;

        public UpdateAuctionStatusesHandlerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }

        public async ValueTask InitializeAsync()
        {
            await _fixture.ResetAsync();

            _notificationService = Substitute.For<IAuctionNotificationService>();
            _notificationService
                .NotifyAuctionStatusChanged(Arg.Any<Guid>(), Arg.Any<string>())
                .Returns(Task.CompletedTask);

            var services = new ServiceCollection();
            services.AddSingleton(_fixture.Context);
            services.AddScoped<IAuctionRepository, AuctionRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton(_notificationService);
            services.AddScoped<ILogger<UpdateAuctionStatusesHandler>>(sp => Substitute.For<ILogger<UpdateAuctionStatusesHandler>>());
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateAuctionStatusesHandler).Assembly));

            var provider = services.BuildServiceProvider();
            _mediator = provider.GetRequiredService<IMediator>();
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task Handle_WhenAuctionStartDateHasPassed_ShouldChangeStatusToOpen()
        {
            // Arrange
            var auctioneer = await SeedAuctioneerAsync(userId: 1);
            var auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.AWAITING_START)
                .WithAuctioneerId(auctioneer.Id)
                .WithStartDate(DateTime.UtcNow.AddMinutes(-10))
                .WithEndDate(DateTime.UtcNow.AddHours(2))
                .Build();
            _fixture.Context.Auctions.Add(auction);
            await _fixture.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            // Act
            await _mediator.Send(new UpdateAuctionStatusesCommand(), TestContext.Current.CancellationToken);

            // Assert
            var updated = await _fixture.Context.Auctions
                .FirstOrDefaultAsync(a => a.Id == auction.Id, TestContext.Current.CancellationToken);
            updated!.Status.Should().Be(AuctionStatus.OPEN);
            await _notificationService.Received(1)
                .NotifyAuctionStatusChanged(auction.ExternalId, Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_WhenAuctionEndDateHasPassed_ShouldChangeStatusToClosed()
        {
            // Arrange
            var auctioneer = await SeedAuctioneerAsync(userId: 1);
            var auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.OPEN)
                .WithAuctioneerId(auctioneer.Id)
                .WithStartDate(DateTime.UtcNow.AddHours(-3))
                .WithEndDate(DateTime.UtcNow.AddMinutes(-5))
                .Build();
            _fixture.Context.Auctions.Add(auction);
            await _fixture.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            // Act
            await _mediator.Send(new UpdateAuctionStatusesCommand(), TestContext.Current.CancellationToken);

            // Assert
            var updated = await _fixture.Context.Auctions
                .FirstOrDefaultAsync(a => a.Id == auction.Id, TestContext.Current.CancellationToken);
            updated!.Status.Should().Be(AuctionStatus.CLOSED);
            await _notificationService.Received(1)
                .NotifyAuctionStatusChanged(auction.ExternalId, Arg.Any<string>());
        }

        [Fact]
        public async Task Handle_WhenAuctionIsStillScheduledForFuture_ShouldNotChangeStatus()
        {
            // Arrange
            var auctioneer = await SeedAuctioneerAsync(userId: 1);
            var auction = new AuctionBuilder()
                .WithStatus(AuctionStatus.AWAITING_START)
                .WithAuctioneerId(auctioneer.Id)
                .WithStartDate(DateTime.UtcNow.AddHours(2))
                .WithEndDate(DateTime.UtcNow.AddHours(5))
                .Build();
            _fixture.Context.Auctions.Add(auction);
            await _fixture.Context.SaveChangesAsync(TestContext.Current.CancellationToken);

            // Act
            await _mediator.Send(new UpdateAuctionStatusesCommand(), TestContext.Current.CancellationToken);

            // Assert
            var updated = await _fixture.Context.Auctions
                .FirstOrDefaultAsync(a => a.Id == auction.Id, TestContext.Current.CancellationToken);
            updated!.Status.Should().Be(AuctionStatus.AWAITING_START);
            await _notificationService.DidNotReceive()
                .NotifyAuctionStatusChanged(Arg.Any<Guid>(), Arg.Any<string>());
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
