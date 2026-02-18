using Microsoft.EntityFrameworkCore;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Repositories
{
    public class BidderRepository: BaseRepository<Bidder>, IBidderRepository
    {
        public BidderRepository(PostgresDbContext context) : base(context)
        {
        }
        public async Task<Bidder?> GetByUserExternalIdAsync(Guid userExternalId)
        {
            return await (from a in _context.Bidders
                          join u in _context.Users on a.Id equals u.Id
                          where u.ExternalId == userExternalId
                          select a)
                              .AsNoTracking()
                              .FirstOrDefaultAsync();
        }
    }
}
