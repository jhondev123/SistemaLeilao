using MediatR;
using Microsoft.Extensions.Logging;
using SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public class CreateAuctionHandler(
        IAuctionRepository auctionRepository,
        IAuctioneerRepository auctioneerRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateAuctionHandler> logger) : IRequestHandler<CreateAuctionCommand, Result<CreateAuctionResponseDto?>>
    {
        public async Task<Result<CreateAuctionResponseDto?>> Handle(CreateAuctionCommand request, CancellationToken ct)
        {
            logger.LogInformation("Iniciando criação de leilão.");

            var auctioneer = await auctioneerRepository.GetByExternalIdAsync(request.auctioneerId);
            if (auctioneer is null)
            {
                logger.LogWarning("Leiloeiro com ID {AuctioneerId} não encontrado.", request.auctioneerId);
                return Result<CreateAuctionResponseDto?>.Failure("Leiloeiro não encontrado.");
            }

            Auction auction = new Auction(
                request.Title,
                auctioneer.Id,
                request.StartingPrice,
                request.StartingPrice,
                request.StartDate,
                request.EndDate,
                request.Description,
                request.Image,
                request.MinimumIncrement,
                Domain.Enums.AuctionStatus.AWAITING_START);

            auctionRepository.Add(auction);

            var result = await unitOfWork.CommitAsync(ct);

            if(result > 0)
            {
                logger.LogInformation("Leilão criado com sucesso. ID do Leilão: {AuctionId}", auction.Id);
                return Result<CreateAuctionResponseDto?>.Success((CreateAuctionResponseDto)auction, "Leilão criado com sucesso.");
            }
            logger.LogError("Erro ao criar leilão.");
            return Result<CreateAuctionResponseDto?>.Failure("Erro ao criar leilão.");
        }
    }
}
