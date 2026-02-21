using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Tests.Common.Builders
{
    public class BidderBuilder
    {
        private string _perfilName = "Arrematante de Teste";
        private long _userId = 1L;
        private Guid _externalId = Guid.NewGuid();
        private decimal _walletBalance = 0m;
        private List<Bid> _bids = new List<Bid>();

        public BidderBuilder WithPerfilName(string perfilName)
        {
            _perfilName = perfilName;
            return this;
        }

        public BidderBuilder WithUserId(long userId)
        {
            _userId = userId;
            return this;
        }

        public BidderBuilder WithExternalId(Guid externalId)
        {
            _externalId = externalId;
            return this;
        }

        public BidderBuilder WithWalletBalance(decimal walletBalance)
        {
            _walletBalance = walletBalance;
            return this;
        }

        public BidderBuilder WithBids(List<Bid> bids)
        {
            _bids = bids;
            return this;
        }

        public Bidder Build()
        {
            Bidder bidder = new Bidder(_perfilName, _userId)
            {
                ExternalId = _externalId,
                Bids = _bids
            };

            if (_walletBalance > 0m)
            {
                bidder.AddCredits(_walletBalance);
            }

            return bidder;
        }
    }
}
