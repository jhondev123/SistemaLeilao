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
            await hubContext.Clients.User(bidderId.ToString())
                    .SendAsync("BidRejected", message);
        }

        public async Task NotifyAuctionStatusChanged(Guid auctionId, string status)
        {
            await hubContext.Clients.Group(auctionId.ToString())
                .SendAsync("StatusChanged", new { auctionId, status });
        }
    }
}
