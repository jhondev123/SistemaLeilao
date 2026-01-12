using Microsoft.EntityFrameworkCore;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Repositories
{
    public class BidRepository(PostgresDbContext context)
    : BaseRepository<Bid>(context), IBidRepository
    {
        public async Task<IEnumerable<Bid>> GetBidsByAuctionIdAsync(long auctionId)
        {
            return await _context.Bids
                    .Where(x => x.AuctionId == auctionId)
                    .ToListAsync();
        }
    }
}
