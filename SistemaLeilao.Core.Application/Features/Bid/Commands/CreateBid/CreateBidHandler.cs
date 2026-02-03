using MediatR;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid
{
    public class CreateBidHandler(
        IBidRepository bidRepository,
        IAuctionRepository auctionRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateBidHandler> logger) : IRequestHandler<CreateBidCommand, Result>
    {
        public async Task<Result> Handle(CreateBidCommand request, CancellationToken ct)
        {
            logger.LogInformation("Iniciando criação de lance.");
            Domain.Entities.Auction? auction = await auctionRepository.GetByIdAsync(request.AuctionId);
            if (auction is null)
            {
                logger.LogWarning("Leilão com ID {AuctionId} não encontrado.", request.AuctionId);
                return Result.Failure("Leilão não encontrado.");
            }

            Domain.Entities.Bid newBid = new(request.Amount, request.BidderId, request.AuctionId);
            auction.UpdatePrice(request.Amount, request.BidderId);

            bidRepository.Add(newBid);
            auctionRepository.Update(auction);

            var result = await unitOfWork.CommitAsync(ct);

            if (result > 0)
            {
                logger.LogInformation("Lance criado com sucesso para o leilão ID {AuctionId}.", request.AuctionId);
                return Result.Success("Lance criado com sucesso.");
            }
            logger.LogError("Erro ao criar lance para o leilão ID {AuctionId}.", request.AuctionId);
            return Result.Failure("Erro ao criar lance.");

        }
    }
}
