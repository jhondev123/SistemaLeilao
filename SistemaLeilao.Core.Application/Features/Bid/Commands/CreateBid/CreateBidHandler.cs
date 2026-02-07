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
        IBidderRepository bidderRepository,
        IAuctionRepository auctionRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateBidHandler> logger) : IRequestHandler<CreateBidCommand, Result<CreateBidResponseDto?>>
    {
        public async Task<Result<CreateBidResponseDto?>> Handle(CreateBidCommand request, CancellationToken ct)
        {
            logger.LogInformation("Iniciando criação de lance.");
            
            Domain.Entities.Auction? auction = await auctionRepository.GetByExternalIdAsync(request.AuctionId);
            if (auction is null)
            {
                logger.LogWarning("Leilão com ID {AuctionId} não encontrado.", request.AuctionId);
                return Result<CreateBidResponseDto?>.Failure("Leilão não encontrado.");
            }

            Domain.Entities.Bidder? bidder = await bidderRepository.GetByExternalIdAsync(request.BidderId);
            if (bidder is null)
            {
                logger.LogWarning("Licitante com ID {BidderId} não encontrado.", request.BidderId);
                return Result<CreateBidResponseDto?>.Failure("Licitante não encontrado.");
            }

            Domain.Entities.Bid newBid = new(request.Amount, bidder.Id, auction.Id);

            var (successApplyNewBid,errorMessageApplyNewBid) = auction.ApplyNewBid(request.Amount, bidder.Id);
            if (!successApplyNewBid)
            {
                logger.LogWarning("Falha ao aplicar novo lance: {ErrorMessage}", errorMessageApplyNewBid);
                return Result<CreateBidResponseDto?>.Failure(errorMessageApplyNewBid);
            }

            bidRepository.Add(newBid);
            auctionRepository.Update(auction);

            var result = await unitOfWork.CommitAsync(ct);

            if (result > 0)
            {
                logger.LogInformation("Lance criado com sucesso para o leilão ID {AuctionId}.", request.AuctionId);
                return Result<CreateBidResponseDto?>.Success(CreateBidResponseDto.EntityToDto(newBid), "Lance criado com sucesso.");
            }
            logger.LogError("Erro ao criar lance para o leilão ID {AuctionId}.", request.AuctionId);
            return Result<CreateBidResponseDto?>.Failure("Erro ao criar lance.");

        }
    }
}
