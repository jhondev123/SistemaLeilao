using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.UpdateAuctionStatuses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.BackgroundServices
{
    public class AuctionStatusWorker(
        IServiceScopeFactory scopeFactory,
        ILogger<AuctionStatusWorker> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Worker de Status de Leilão iniciado.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = scopeFactory.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

                        // Dispara um único comando que resolve abertura e fechamento
                        await mediator.Send(new UpdateAuctionStatusesCommand(), stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Erro ao processar atualização de status dos leilões.");
                }

                // Espera 5 segundos antes da próxima execução
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
