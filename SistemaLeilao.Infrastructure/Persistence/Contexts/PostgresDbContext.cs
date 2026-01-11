using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Infrastructure.Indentity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Net;
using System.Text;

namespace SistemaLeilao.Infrastructure.Persistence.Contexts
{
    public class PostgresDbContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>
    {
        #region [ DBSETS ]

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Bidder> Bidders { get; set; }
        public DbSet<Bid> Bids { get; set; }

        #endregion

        public PostgresDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            RenameIndetityTables(modelBuilder);

            ConfiguringBaseEntities(modelBuilder);

            // busca todas as configurações de entidade na assembly, na pasta de Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PostgresDbContext).Assembly);

        }
        private void RenameIndetityTables(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity => { entity.ToTable(name: "Users"); });
            modelBuilder.Entity<Role>(entity => { entity.ToTable(name: "Roles"); });
            modelBuilder.Entity<RoleClaim>(entity => { entity.ToTable(name: "RoleClaims"); });
            modelBuilder.Entity<UserClaim>(entity => { entity.ToTable(name: "UserClaims"); });
            modelBuilder.Entity<UserLogin>(entity => { entity.ToTable(name: "UserLogins"); });
            modelBuilder.Entity<UserRole>(entity => { entity.ToTable(name: "UserRoles"); });
            modelBuilder.Entity<UserToken>(entity => { entity.ToTable(name: "UserTokens"); });
        }
        private void ConfiguringBaseEntities(ModelBuilder modelBuilder)
        {
            foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
            {
                ApplyQueryFilterSoftDelete(modelBuilder, entityType);

                ConfiguringTimestampFields(modelBuilder, entityType);

                ConfiguringSoftDeleteField(modelBuilder, entityType);
            }
        }
        private void ApplyQueryFilterSoftDelete(ModelBuilder modelBuilder, IMutableEntityType entityType)
        {
            // Aplicar QueryFilter para soft delete
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt));
                var filter = Expression.Lambda(
                    Expression.Equal(property, Expression.Constant(null, typeof(DateTime?))),
                    parameter
                );

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
        private void ConfiguringTimestampFields(ModelBuilder modelBuilder, IMutableEntityType entityType)
        {
            // Configurar timestamps
            if (typeof(ITimestampEntity).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(ITimestampEntity.CreatedAt))
                    .HasColumnType("timestamp with time zone");

                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(ITimestampEntity.UpdatedAt))
                    .HasColumnType("timestamp with time zone");
            }
        }
        private void ConfiguringSoftDeleteField(ModelBuilder modelBuilder, IMutableEntityType entityType)
        {
            // Configurar DeletedAt
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(ISoftDeletable.DeletedAt))
                    .HasColumnType("timestamp with time zone")
                    .IsRequired(false);
            }
        }

        #region [ SOBREESCRITAS DE SAVECHANGES ]
        public override int SaveChanges()
        {
            HandleSoftDelete();
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            HandleSoftDelete();
            UpdateTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<ITimestampEntity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
        private void HandleSoftDelete()
        {
            var entries = ChangeTracker.Entries<ISoftDeletable>()
                .Where(e => e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                entry.State = EntityState.Modified;
                entry.Entity.DeletedAt = DateTime.UtcNow;
            }
        }
        #endregion
    }
}
