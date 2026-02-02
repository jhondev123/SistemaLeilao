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
            var logger = services.GetRequiredService<ILogger<PostgresDbContext>>();

            var configuration = services.GetRequiredService<IConfiguration>();

            try
            {

                var context = services.GetRequiredService<PostgresDbContext>();
                logger.LogInformation("Rodando as migrations do banco de dados");
                await context.Database.MigrateAsync();

                logger.LogInformation("Rodando o IdentitySeeder");
                await IdentitySeeder.SeedRolesAndAdminAsync(services, configuration);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Ocorreu um erro durante a migração ou seeding do banco.");
                throw;
            }
        }
    }
}
