using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Tests.Common.Builders
{
    public class AuctionBuilder
    {
        private string _title = "Leilão de Teste";
        private decimal _startingPrice = 100m;
        private decimal _currentPrice = 100m;
        private decimal _minimumIncrement = 10m;
        private DateTime _startDate = DateTime.UtcNow.AddHours(-1);
        private DateTime _endDate = DateTime.UtcNow.AddHours(2);
        private AuctionStatus _status = AuctionStatus.OPEN;
        private long _auctioneerId = 1;
        private Guid _externalId = Guid.NewGuid();

        public AuctionBuilder WithStatus(AuctionStatus status)
        {
            _status = status;
            return this;
        }

        public AuctionBuilder WithEndDate(DateTime endDate)
        {
            _endDate = endDate;
            return this;
        }

        public AuctionBuilder WithStartDate(DateTime startDate)
        {
            _startDate = startDate;
            return this;
        }

        public AuctionBuilder WithCurrentPrice(decimal price)
        {
            _currentPrice = price;
            return this;
        }

        public AuctionBuilder WithMinimumIncrement(decimal increment)
        {
            _minimumIncrement = increment;
            return this;
        }

        public AuctionBuilder WithAuctioneerId(long auctioneerId)
        {
            _auctioneerId = auctioneerId;
            return this;
        }

        public AuctionBuilder WithExternalId(Guid externalId)
        {
            _externalId = externalId;
            return this;
        }

        public Auction Build()
        {
            Auction auction = new Auction(
                _title, _auctioneerId, _startingPrice, _currentPrice,
                _startDate, _endDate, null, null, _minimumIncrement, _status);

            auction.ExternalId = _externalId;

            return auction;
        }
    }
}
