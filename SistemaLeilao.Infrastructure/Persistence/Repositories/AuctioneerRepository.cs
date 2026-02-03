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
    }
}
