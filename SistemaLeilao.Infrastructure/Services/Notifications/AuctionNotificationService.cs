using Microsoft.AspNetCore.SignalR;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Infrastructure.Hubs;

namespace SistemaLeilao.Infrastructure.Services.Notifications
{
    public class AuctionNotificationService(IHubContext<AuctionHub> hubContext) : IAuctionNotificationService
    {
        public async Task NotifyNewBid(Guid auctionId, decimal newPrice, Guid bidderId)
        {
            // Envia para o "Grupo" do leilão no SignalR
            await hubContext.Clients.Group(auctionId.ToString())
                .SendAsync("NewBidReceived", new { auctionId, newPrice, bidderId });
        }

        public async Task NotifyBidRejected(Guid bidderId, string message)
        {
            // Aqui você enviaria para um usuário específico
            // (Exige mapeamento de ConnectionId ou User Identifier)
            await hubContext.Clients.User(bidderId.ToString()).SendAsync("Error", message);
        }

        public async Task NotifyAuctionStatusChanged(Guid auctionId, string status)
        {
            await hubContext.Clients.Group(auctionId.ToString())
                .SendAsync("StatusChanged", new { auctionId, status });
        }
    }
}
