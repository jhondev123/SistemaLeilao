using SistemaLeilao.Core.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Entities
{
    public class Bidder : PersonBaseEntity
    {
        public string PerfilName { get; set; }
        public string PhoneNumber { get; set; }
        public decimal WalletBalance { get; private set; }
        public List<Auction> Auctions { get; set; }
        public List<Bid> Bids { get; set; }
        public void AddCredits(decimal amount) => WalletBalance += amount;
    }
}
