using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Interfaces.Repositories
{
    public interface IBidRepository : IRepository<Bid>
    {
        Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(long auctionId);
        void Add(Bid bid);
    }
}
