using Microsoft.EntityFrameworkCore;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Repositories
{
    public class AuctioneerRepository(PostgresDbContext context)
    : BaseRepository<Auctioneer>(context), IAuctioneerRepository
    {
        public async Task<Auctioneer?> GetByUserExternalIdAsync(Guid userExternalId)
        {
            return await (from a in context.Auctioneers
                          join u in context.Users on a.Id equals u.Id
                          where u.ExternalId == userExternalId
                          select a)
                              .AsNoTracking()
                              .FirstOrDefaultAsync();
        }
    }
}
