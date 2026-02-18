using MassTransit;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid;
using SistemaLeilao.Core.Application.Features.Bid.Events;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Services.Bid;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Consumers
{
    public class BidPlacedConsumer(
        IAuctionNotificationService notificationService,
        IAuctionRepository auctionRepository,
        IBidderRepository bidderRepository,
        IUnitOfWork unitOfWork,
        BidDomainService bidDomainService,
        ILogger<BidPlacedConsumer> logger) : IConsumer<BidPlacedEvent>
    {

        public async Task Consume(ConsumeContext<BidPlacedEvent> context)
        {
            var request = context.Message;
            var ct = context.CancellationToken;

            logger.LogInformation("Processando lance de {Amount} para o leilão {AuctionId}.", request.Amount, request.AuctionId);

            var auction = await auctionRepository.GetByExternalIdAsync(request.AuctionId);
            var bidder = await bidderRepository.GetByExternalIdAsync(request.BidderId);

            var validationResult = bidDomainService.ValidateBid(auction, bidder, request.Amount);

            if (!validationResult.success)
            {
                await notificationService.NotifyBidRejected(request.UserExternalId, validationResult.error);
                return;
            }

            var (success, errorMessage) = auction!.ApplyNewBid(request.Amount, bidder!.Id);

            if (!success)
            {
                logger.LogWarning("Lance de {Amount} rejeitado para o leilão {AuctionId}: {Reason}",
                    request.Amount, request.AuctionId, errorMessage);

                await notificationService.NotifyBidRejected(request.UserExternalId, errorMessage);
                return;
            }

            var newBid = new Domain.Entities.Bid(request.Amount, auction, bidder);
            auction.Bids.Add(newBid);

            auctionRepository.Update(auction);

            try
            {
                await unitOfWork.CommitAsync(ct);
                logger.LogInformation("Lance de {Amount} confirmado com sucesso!", request.Amount);

                await notificationService.NotifyNewBid(auction.ExternalId, request.Amount, request.UserExternalId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro de infraestrutura ao salvar lance.");
                throw;
            }
        }

    }
}
