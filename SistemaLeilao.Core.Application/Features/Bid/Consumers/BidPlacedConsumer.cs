using MassTransit;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid;
using SistemaLeilao.Core.Application.Features.Bid.Events;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Consumers
{
    public class BidPlacedConsumer(
        IAuctionRepository auctionRepository,
        IBidderRepository bidderRepository,
        IBidRepository bidRepository,
        IUnitOfWork unitOfWork,
        ILogger<BidPlacedConsumer> logger) : IConsumer<BidPlacedEvent>
    {

        public async Task Consume(ConsumeContext<BidPlacedEvent> context)
        {
            var request = context.Message;
            var ct = context.CancellationToken;

            logger.LogInformation("Processando lance de {Amount} para o leilão {AuctionId}.", request.Amount, request.AuctionId);

            // 1. Busca as entidades usando o ExternalId (Guid)
            var auction = await auctionRepository.GetByExternalIdAsync(request.AuctionId);
            if (auction is null)
            {
                logger.LogWarning("Leilão {AuctionId} não encontrado. Processamento abortado.", request.AuctionId);
                return; // Mensagem sai da fila (ACK) pois o erro não é recuperável
            }

            var bidder = await bidderRepository.GetByExternalIdAsync(request.BidderId);
            if (bidder is null)
            {
                logger.LogWarning("Licitante {BidderId} não encontrado.", request.BidderId);
                return;
            }

            // 2. Tenta aplicar a regra de negócio no Domínio
            var (success, errorMessage) = auction.ApplyNewBid(request.Amount, bidder.Id);

            if (!success)
            {
                logger.LogWarning("Lance de {Amount} rejeitado para o leilão {AuctionId}: {Reason}",
                    request.Amount, request.AuctionId, errorMessage);

                // TODO: Notificar o usuário via SignalR que o lance dele foi recusado
                return;
            }

            // 3. Persistência
            var newBid = new Domain.Entities.Bid(request.Amount, bidder.Id, auction.Id);
            bidRepository.Add(newBid);

            // O Update é opcional se o EF já estiver rastreando a entidade 'auction', 
            // mas ajuda na legibilidade do repositório
            auctionRepository.Update(auction);

            try
            {
                await unitOfWork.CommitAsync(ct);
                logger.LogInformation("Lance de {Amount} confirmado com sucesso!", request.Amount);

                // TODO: Notificar via SignalR:
                // 1. O vencedor atual (Confirmação)
                // 2. Todos os outros (Novo preço no leilão)
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro de infraestrutura ao salvar lance.");
                // Se dispararmos uma exceção aqui, o MassTransit faz o Retry automático
                throw;
            }
        }

    }
}
