using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork(PostgresDbContext context) : IUnitOfWork
    {
        public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }

        public void Dispose() => context.Dispose();
    }
}
