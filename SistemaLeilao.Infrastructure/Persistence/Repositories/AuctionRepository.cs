using Microsoft.EntityFrameworkCore;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Repositories
{
    public class AuctionRepository(PostgresDbContext context)
    : BaseRepository<Auction>(context), IAuctionRepository
    {
        public async Task<IEnumerable<Auction>> GetAuctionsToCloseAsync(DateTime date)
        {
            return await _context.Auctions
                .Where(a => a.EndDate <= date && a.Status == AuctionStatus.OPEN)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auction>> GetAuctionsToOpenAsync(DateTime date)
        {
            return await _context.Auctions
                .Where(a => a.StartDate <= date && a.Status == AuctionStatus.AWAITING_START)
                .ToListAsync();

        }

        public async Task<Auction?> GetAuctionWithBidsAsync(long id)
        {
            return await _context.Auctions
                .Include(a => a.Bids)
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
