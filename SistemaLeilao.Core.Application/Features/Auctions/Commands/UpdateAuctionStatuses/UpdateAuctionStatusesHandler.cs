using MediatR;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Common.Extensions;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auctions.Commands.UpdateAuctionStatuses
{
    public class UpdateAuctionStatusesHandler(
        IAuctionRepository auctionRepository,
        IAuctionNotificationService notificationService,
        IUnitOfWork unitOfWork,
        ILogger<UpdateAuctionStatusesHandler> logger) : IRequestHandler<UpdateAuctionStatusesCommand>
    {
        public async Task Handle(UpdateAuctionStatusesCommand request, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            logger.LogInformation("iniciando processamento dos status dos leilões {now}", now);

            var auctionsToOpen = await auctionRepository.GetAuctionsToOpenAsync(now);
            foreach (var auction in auctionsToOpen)
            {
                auction.Status = AuctionStatus.OPEN;
                logger.LogInformation("Leilão {Id} foi ABERTO.", auction.Id);
            }

            var auctionsToClose = await auctionRepository.GetAuctionsToCloseAsync(now);
            foreach (var auction in auctionsToClose)
            {
                auction.Status = AuctionStatus.CLOSED;
                logger.LogInformation("Leilão {Id} foi ENCERRADO.", auction.Id);
            }

            if (auctionsToOpen.Any() || auctionsToClose.Any())
            {
                await unitOfWork.CommitAsync(ct);

                foreach (var auction in auctionsToOpen)
                {
                    await notificationService.NotifyAuctionStatusChanged(auction.ExternalId, AuctionStatus.OPEN.GetDescription());
                }

                foreach (var auction in auctionsToClose)
                {
                    await notificationService.NotifyAuctionStatusChanged(auction.ExternalId, AuctionStatus.CLOSED.GetDescription());
                }
            }
        }
    }
}
