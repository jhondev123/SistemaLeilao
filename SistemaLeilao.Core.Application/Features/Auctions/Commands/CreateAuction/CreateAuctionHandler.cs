using MediatR;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public class CreateAuctionHandler(
        IAuctionRepository auctionRepository,
        IUnitOfWork unitOfWork,
        IUserContextService userContextService,
        ILogger<CreateAuctionHandler> logger) : IRequestHandler<CreateAuctionCommand, Result<CreateAuctionResponseDto?>>
    {
        public async Task<Result<CreateAuctionResponseDto?>> Handle(CreateAuctionCommand request, CancellationToken ct)
        {
            logger.LogInformation("Iniciando criação de leilão.");

            var auctioneer = await userContextService.GetCurrentAuctioneerAsync();
            if (auctioneer is null)
            {
                logger.LogWarning("Leiloeiro não encontrado.");
                return Result<CreateAuctionResponseDto?>.Failure(new ErrorMessage(nameof(Messages.ErrorAuctioneerNotFound),Messages.ErrorAuctioneerNotFound));
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
