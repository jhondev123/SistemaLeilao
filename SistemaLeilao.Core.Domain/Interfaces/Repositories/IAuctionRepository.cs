using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Interfaces.Repositories
{
    public interface IAuctionRepository : IRepository<Auction>
    {
        Task<Auction?> GetAuctionWithBidsAsync(long id);
    }
}
