using Microsoft.EntityFrameworkCore;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Repositories
{
    public class BidderRepository(PostgresDbContext context)
    : BaseRepository<Bidder>(context), IBidderRepository
    {
        public async Task<Bidder?> GetByUserExternalIdAsync(Guid userExternalId)
        {
            return await (from a in context.Bidders
                          join u in context.Users on a.Id equals u.Id
                          where u.ExternalId == userExternalId
                          select a)
                              .AsNoTracking()
                              .FirstOrDefaultAsync();
        }
    }
}
