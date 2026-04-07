using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.UpdateAuctionStatuses;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Tests.Common.Builders;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLeilao.UnitTests.Application.Features.Auctions
{
    public class UpdateAuctionStatusesHandlerTests
    {
        private readonly Mock<IAuctionRepository> _auctionRepositoryMock;
        private readonly Mock<IAuctionNotificationService> _notificationServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdateAuctionStatusesHandler>> _loggerMock;
        private readonly UpdateAuctionStatusesHandler _handler;

        public UpdateAuctionStatusesHandlerTests()
        {
            _auctionRepositoryMock = new Mock<IAuctionRepository>();
            _notificationServiceMock = new Mock<IAuctionNotificationService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateAuctionStatusesHandler>>();

            _handler = new UpdateAuctionStatusesHandler(
                _auctionRepositoryMock.Object,
                _notificationServiceMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_WhenNoAuctionsToProcess_ShouldNotCommit()
        {
            // Arrange
            _auctionRepositoryMock
                .Setup(r => r.GetAuctionsToOpenAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Auction>());
            _auctionRepositoryMock
                .Setup(r => r.GetAuctionsToCloseAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Auction>());

            // Act
            await _handler.Handle(new UpdateAuctionStatusesCommand(), CancellationToken.None);

            // Assert
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _notificationServiceMock.Verify(n => n.NotifyAuctionStatusChanged(It.IsAny<Guid>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task Handle_WhenAuctionsNeedToOpen_ShouldUpdateStatusAndCommit()
        {
            // Arrange
            Auction auctionToOpen = new AuctionBuilder()
                .WithStatus(AuctionStatus.AWAITING_START)
                .Build();

            _auctionRepositoryMock
                .Setup(r => r.GetAuctionsToOpenAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Auction> { auctionToOpen });
            _auctionRepositoryMock
                .Setup(r => r.GetAuctionsToCloseAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Auction>());
            _unitOfWorkMock
                .Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _notificationServiceMock
                .Setup(n => n.NotifyAuctionStatusChanged(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(new UpdateAuctionStatusesCommand(), CancellationToken.None);

            // Assert
            auctionToOpen.Status.Should().Be(AuctionStatus.OPEN);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _notificationServiceMock.Verify(
                n => n.NotifyAuctionStatusChanged(auctionToOpen.ExternalId, It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenAuctionsNeedToClose_ShouldUpdateStatusAndCommit()
        {
            // Arrange
            Auction auctionToClose = new AuctionBuilder()
                .WithStatus(AuctionStatus.OPEN)
                .Build();

            _auctionRepositoryMock
                .Setup(r => r.GetAuctionsToOpenAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Auction>());
            _auctionRepositoryMock
                .Setup(r => r.GetAuctionsToCloseAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Auction> { auctionToClose });
            _unitOfWorkMock
                .Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);
            _notificationServiceMock
                .Setup(n => n.NotifyAuctionStatusChanged(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(new UpdateAuctionStatusesCommand(), CancellationToken.None);

            // Assert
            auctionToClose.Status.Should().Be(AuctionStatus.CLOSED);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _notificationServiceMock.Verify(
                n => n.NotifyAuctionStatusChanged(auctionToClose.ExternalId, It.IsAny<string>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenBothTypesOfAuctionExist_ShouldCommitOnlyOnce()
        {
            // Arrange
            Auction auctionToOpen = new AuctionBuilder().WithStatus(AuctionStatus.AWAITING_START).Build();
            Auction auctionToClose = new AuctionBuilder().WithStatus(AuctionStatus.OPEN).Build();

            _auctionRepositoryMock
                .Setup(r => r.GetAuctionsToOpenAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Auction> { auctionToOpen });
            _auctionRepositoryMock
                .Setup(r => r.GetAuctionsToCloseAsync(It.IsAny<DateTime>()))
                .ReturnsAsync(new List<Auction> { auctionToClose });
            _unitOfWorkMock
                .Setup(uow => uow.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(2);
            _notificationServiceMock
                .Setup(n => n.NotifyAuctionStatusChanged(It.IsAny<Guid>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _handler.Handle(new UpdateAuctionStatusesCommand(), CancellationToken.None);

            // Assert
            auctionToOpen.Status.Should().Be(AuctionStatus.OPEN);
            auctionToClose.Status.Should().Be(AuctionStatus.CLOSED);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _notificationServiceMock.Verify(
                n => n.NotifyAuctionStatusChanged(It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Exactly(2));
        }
    }
}
