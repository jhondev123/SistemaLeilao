using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Persistence.Repositories;
using System.Security.Claims;

namespace SistemaLeilao.Infrastructure.Hubs
{
    [Authorize]
    public class AuctionHub(
        ILogger<AuctionHub> logger,
        IAuctionRepository auctionRepository
        ) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            await base.OnConnectedAsync();
        }
        public async Task JoinAuction(string auctionId)
        {
            logger.LogInformation("chegou no join auction do hub");
            if (!Guid.TryParse(auctionId, out var auctionGuid))
            {
                await Clients.Caller.SendAsync("Error", "ID de leilão inválido.");
                return;
            }

            var auction = await auctionRepository.GetByExternalIdAsync(auctionGuid);
            if (auction is null)
            {
                await Clients.Caller.SendAsync("Error", "Leilão não encontrado.");
                return;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, auctionId);
            await Clients.Caller.SendAsync("JoinedAuction", $"Você entrou no leilão: {auctionId}");
        }

        public async Task LeaveAuction(string auctionId)
        {
            logger.LogInformation("chegou no leave auction do hub");
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, auctionId);
        }
    }
}
