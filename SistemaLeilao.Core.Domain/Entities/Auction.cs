using SistemaLeilao.Core.Domain.Entities.Common;
using SistemaLeilao.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Entities
{
    public class Auction : BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public AuctionStatus Status { get; set; }
        public byte[]? Image { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal StartingPrice { get; set; } = 0;
        public decimal CurrentPrice { get; set; } = 0;
        public decimal MinimumIncrement { get; set; } = 0;
        public long? BidderWinnerId { get; set; }
        public Bidder? BidderWinner { get; set; }
        public List<Bid> Bids { get; set; }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice > MinimumIncrement && newPrice > CurrentPrice)
            {
                CurrentPrice = newPrice;
            }
        }
    }
}
