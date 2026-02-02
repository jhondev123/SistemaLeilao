using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using SistemaLeilao.Infrastructure.Persistence.Seeders;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Infrastructure.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrations(this IHost app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();

            try
            {
                var context = services.GetRequiredService<PostgresDbContext>();
                await context.Database.MigrateAsync();

                await IdentitySeeder.SeedRolesAndAdminAsync(services, configuration);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger>();
                logger.LogError(ex, "Ocorreu um erro durante a migração ou seeding do banco.");
                throw;
            }
        }
    }
}
