using MediatR;
using Microsoft.Extensions.Logging;
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
        IUnitOfWork unitOfWork,
        ILogger<UpdateAuctionStatusesHandler> logger) : IRequestHandler<UpdateAuctionStatusesCommand>
    {
        public async Task Handle(UpdateAuctionStatusesCommand request, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            // 1. Abrir leilões (AWAITING_START -> OPEN)
            // Dica: Crie um método no repositório para buscar com esses filtros
            var auctionsToOpen = await auctionRepository.GetAuctionsToOpenAsync(now);
            foreach (var auction in auctionsToOpen)
            {
                auction.Status = AuctionStatus.OPEN;
                logger.LogInformation("Leilão {Id} foi ABERTO.", auction.Id);
            }

            // 2. Encerrar leilões (OPEN -> CLOSED)
            var auctionsToClose = await auctionRepository.GetAuctionsToCloseAsync(now);
            foreach (var auction in auctionsToClose)
            {
                auction.Status = AuctionStatus.CLOSED;
                // Aqui você poderia identificar o vencedor final
                logger.LogInformation("Leilão {Id} foi ENCERRADO.", auction.Id);
            }

            if (auctionsToOpen.Any() || auctionsToClose.Any())
            {
                await unitOfWork.CommitAsync(ct);
            }
        }
    }
}
