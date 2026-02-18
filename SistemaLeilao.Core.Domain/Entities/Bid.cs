using SistemaLeilao.Core.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Entities
{
    public class Bid : BaseEntity
    {
        public decimal Amount { get; set; }
        public DateTime BidDate { get; set; }
        public long AuctionId { get; set; }
        public Auction Auction { get; set; } = new Auction();
        public long BidderId { get; set; }
        public Bidder Bidder { get; set; } = new Bidder();
        public Bid() { }

        public Bid(decimal amount, Auction auction, Bidder bidder)
        {
            Amount = amount;
            Auction = auction;
            Bidder = bidder;
            BidDate = DateTime.UtcNow;
        }
    }
}
