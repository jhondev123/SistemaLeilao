using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Application.Features.Bid.Events;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid
{
    public class CreateBidHandler(
        IPublishEndpoint publishEndpoint,
        ILogger<CreateBidHandler> logger) : IRequestHandler<CreateBidCommand, Result>
    {
        public async Task<Result> Handle(CreateBidCommand request, CancellationToken ct)
        {

            await publishEndpoint.Publish(new BidPlacedEvent(
            request.AuctionId,
            request.BidderId,
            request.Amount), ct);

            logger.LogInformation("Lance enviado para processamento AuctionId: {AuctionId}, BidderId: {BidderId}, Amount: {Amount}",
                request.AuctionId, request.BidderId, request.Amount);
            return Result.Success("Lance enviado para processamento!");
        }
    }
}
