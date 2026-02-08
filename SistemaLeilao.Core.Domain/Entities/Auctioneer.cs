using SistemaLeilao.Core.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Entities
{
    public class Auctioneer : PersonBaseEntity
    {
        public string? Bio { get; set; }
        public List<Auction> Auctions { get; set; } = new();
        public double Rating { get; set; } = 5.0;
        public Auctioneer() { }
        public Auctioneer(string name, string email, long userId)
        {
            Name = name;
            Email = email;
            Id = userId;
        }
    }
}
