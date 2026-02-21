using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Common.Extensions;
using SistemaLeilao.Core.Application.Features.Admin.Commands.CreateUser;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Resources;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace SistemaLeilao.UnitTests.Application.Features.Admin
{
    public class CreateUserHandlerTests
    {
        private readonly Mock<IUnitOfWork> unitOfWorkMock;
        private readonly Mock<IAuthService> authServiceMock;
        private readonly Mock<IAuctioneerRepository> auctioneerRepositoryMock;
        private readonly Mock<IBidderRepository> bidderRepositoryMock;
        private readonly Mock<ILogger<CreateUserHandler>> loggerMock;
        private readonly CreateUserHandler handler;

        public CreateUserHandlerTests()
        {
            unitOfWorkMock = new Mock<IUnitOfWork>();
            authServiceMock = new Mock<IAuthService>();
            auctioneerRepositoryMock = new Mock<IAuctioneerRepository>();
            bidderRepositoryMock = new Mock<IBidderRepository>();
            loggerMock = new Mock<ILogger<CreateUserHandler>>();

            handler = new CreateUserHandler(
                unitOfWorkMock.Object,
                authServiceMock.Object,
                auctioneerRepositoryMock.Object,
                bidderRepositoryMock.Object,
                loggerMock.Object);
        }

        [Fact]
        public async Task Handle_Should_CreateAuctioneer_When_RoleIsAuctioneer()
        {
            // Arrange
            long userId = 10L;
            CreateUserCommand command = new CreateUserCommand(
                "Auctioneer User",
                "auctioneer@example.com",
                "P@ssw0rd123!",
                RoleEnum.Auctioneer.GetDescription());

            authServiceMock
                .Setup(service => service.CreateUserAsync(command.Name, command.Email, command.Password, command.Role))
                .ReturnsAsync((true, new List<string>(), userId));

            unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            Result result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result?.Message?.Message.Should().Be(Messages.SuccessUserCreated);
            result?.Message?.Code.Should().Be(nameof(Messages.SuccessUserCreated));
            auctioneerRepositoryMock.Verify(repository => repository.Add(It.Is<Auctioneer>(
                auctioneer => auctioneer.Id == userId && auctioneer.Email == command.Email && auctioneer.Name == command.Name)), Times.Once);
            bidderRepositoryMock.Verify(repository => repository.Add(It.IsAny<Bidder>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_CreateBidder_When_RoleIsBidder()
        {
            // Arrange
            long userId = 12L;
            CreateUserCommand command = new CreateUserCommand(
                "Bidder User",
                "bidder@example.com",
                "P@ssw0rd123!",
                RoleEnum.Bidder.GetDescription());

            authServiceMock
                .Setup(service => service.CreateUserAsync(command.Name, command.Email, command.Password, command.Role))
                .ReturnsAsync((true, new List<string>(), userId));

            unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            Result result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result?.Message?.Message.Should().Be(Messages.SuccessUserCreated);
            result?.Message?.Code.Should().Be(nameof(Messages.SuccessUserCreated));
            bidderRepositoryMock.Verify(repository => repository.Add(It.Is<Bidder>(
                bidder => bidder.Id == userId && bidder.PerfilName == command.Name)), Times.Once);
            auctioneerRepositoryMock.Verify(repository => repository.Add(It.IsAny<Auctioneer>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_ReturnFailure_When_AuthServiceFails()
        {
            // Arrange
            List<string> errors = new List<string> { Messages.ErrorFailToCreateUser };
            CreateUserCommand command = new CreateUserCommand(
                "User",
                "user@example.com",
                "P@ssw0rd123!",
                RoleEnum.Bidder.GetDescription());

            authServiceMock
                .Setup(service => service.CreateUserAsync(command.Name, command.Email, command.Password, command.Role))
                .ReturnsAsync((false, errors, null));

            // Act
            Result result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors.Should().Contain(x => x.Message == Messages.ErrorFailToCreateUser);
            unitOfWorkMock.Verify(unitOfWork => unitOfWork.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            bidderRepositoryMock.Verify(repository => repository.Add(It.IsAny<Bidder>()), Times.Never);
            auctioneerRepositoryMock.Verify(repository => repository.Add(It.IsAny<Auctioneer>()), Times.Never);
        }
    }
}
