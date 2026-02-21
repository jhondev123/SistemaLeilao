using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Tests.Common.Builders
{
    public class BidBuilder
    {
        private decimal _amount = 150m;
        private Auction _auction = new AuctionBuilder().Build();
        private Bidder _bidder = new BidderBuilder().Build();

        public BidBuilder WithAmount(decimal amount)
        {
            _amount = amount;
            return this;
        }

        public BidBuilder WithAuction(Auction auction)
        {
            _auction = auction;
            return this;
        }

        public BidBuilder WithBidder(Bidder bidder)
        {
            _bidder = bidder;
            return this;
        }

        public Bid Build()
        {
            return new Bid(_amount, _auction, _bidder);
        }
    }
}
