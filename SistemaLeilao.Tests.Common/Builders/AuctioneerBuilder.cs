using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Tests.Common.Builders
{
    public class AuctioneerBuilder
    {
        private string _name = "João Silva";
        private string _email = "joao@example.com";
        private long _userId = 1L;
        private double Rating = 5;
        private List<Auction> _auctions = new();
        private string? _bio = "Leiloeiro experiente com mais de 10 anos no mercado.";

        public AuctioneerBuilder WithName(string name)
        { _name = name; return this; }

        public AuctioneerBuilder WithEmail(string email)
        { _email = email; return this; }

        public AuctioneerBuilder WithUserId(long userId)
        { _userId = userId; return this; }

        public AuctioneerBuilder WithRating(double rating)
        { Rating = rating; return this; }

        public AuctioneerBuilder WithAuctions(List<Auction> auctions)
        { _auctions = auctions; return this; }

        public AuctioneerBuilder WithBio(string? bio)
        { _bio = bio; return this; }

        public Auctioneer Build()
        {
            return new Auctioneer
            {
                Name = _name,
                Email = _email,
                Id = _userId,
                Rating = Rating,
                Auctions = _auctions,
                Bio = _bio
            };
        }
    }
}
