using Microsoft.EntityFrameworkCore;
using SistemaLeilao.Core.Domain.Entities.Common;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Repositories
{
    public abstract class BaseRepository<T>(PostgresDbContext context) : IRepository<T> where T : BaseEntity
    {
        protected readonly PostgresDbContext _context = context;

        public async Task<T?> GetByIdAsync(long id) => await _context.Set<T>().FindAsync(id);

        public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

        public void Add(T entity) => _context.Set<T>().Add(entity);

        public void Update(T entity) => _context.Set<T>().Update(entity);

        public void Remove(T entity) => _context.Set<T>().Remove(entity);

        public async Task<T?> GetByExternalIdAsync(Guid id) => await _context.Set<T>().FirstOrDefaultAsync(x => x.ExternalId == id);

    }
}
