using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Interfaces
{
    public interface IAuctionNotificationService
    {
        // Notifica todos que o preço do leilão mudou
        Task NotifyNewBid(Guid auctionId, decimal newPrice, Guid bidderId);

        // Notifica um usuário específico que o lance dele foi rejeitado
        Task NotifyBidRejected(Guid bidderId, string message);

        // Notifica que o status do leilão mudou (Aberto/Fechado)
        Task NotifyAuctionStatusChanged(Guid auctionId, string status);
    }
}
