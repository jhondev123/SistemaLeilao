using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Application.Features.Bid.Events;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Resources;
using SistemaLeilao.Core.Domain.Services.Bid;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid
{
    public class CreateBidHandler(
        IPublishEndpoint publishEndpoint,
        IUserContextService userContextService,
        ILogger<CreateBidHandler> logger,
        IAuctionRepository auctionRepository,
        BidDomainService bidDomainService) : IRequestHandler<CreateBidCommand, Result>
    {
        public async Task<Result> Handle(CreateBidCommand request, CancellationToken ct)
        {
            var userExternalId = userContextService.GetUserExternalId();
            var bidder = await userContextService.GetCurrentBidderAsync();
            var auction = await auctionRepository.GetByExternalIdAsync(request.AuctionId);
            if (bidder is null)
            {
                logger.LogWarning("Usuário não autenticado tentou fazer um lance. AuctionId: {AuctionId}, Amount: {Amount}",
                    request.AuctionId, request.Amount);
                return Result.Failure(new ErrorMessage(nameof(Messages.ErrorUserNotAuthenticated), Messages.ErrorUserNotAuthenticated));
            }

            var result = bidDomainService.ValidateBid(auction, bidder, request.Amount);

            if (!result.success)
            {
                return Result.Failure(result.error);
            }

            await publishEndpoint.Publish(new BidPlacedEvent(
                request.AuctionId,
                bidder.ExternalId,
                userExternalId,
                request.Amount), ct);

            logger.LogInformation("Lance enviado para processamento AuctionId: {AuctionId}, BidderId: {BidderId}, Amount: {Amount}",
                request.AuctionId, bidder.ExternalId, request.Amount);
            return Result.Success(new SucessMessage(nameof(Messages.SucessBidSentToProcessing),Messages.SucessBidSentToProcessing));
        }
    }
}
