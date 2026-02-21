using Microsoft.EntityFrameworkCore;
using Npgsql;
using Respawn;
using SistemaLeilao.Infrastructure.Persistence.Contexts;
using Testcontainers.PostgreSql;
using Xunit;

namespace SistemaLeilao.IntegrationTests.Fixtures
{
    public class DatabaseFixture : IAsyncLifetime
    {
        private readonly PostgreSqlContainer _container;
        private Respawner _respawner = null!;

        public PostgresDbContext Context { get; private set; } = null!;
        public string ConnectionString { get; private set; } = null!;

        public DatabaseFixture()
        {
            _container = new PostgreSqlBuilder("postgres:15.1")
                .WithDatabase("sistemaLeilao_tests")
                .WithUsername("postgres")
                .WithPassword("postgres")
                .Build();
        }

        public async ValueTask InitializeAsync()
        {
            await _container.StartAsync();
            ConnectionString = _container.GetConnectionString();

            var options = new DbContextOptionsBuilder<PostgresDbContext>()
                .UseNpgsql(ConnectionString)
                .Options;

            Context = new PostgresDbContext(options);
            await Context.Database.MigrateAsync();

            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                TablesToIgnore = ["__EFMigrationsHistory"]
            });
        }

        public async Task ResetAsync()
        {
            await using var conn = new NpgsqlConnection(ConnectionString);
            await conn.OpenAsync();
            await _respawner.ResetAsync(conn);
        }

        public async ValueTask DisposeAsync()
        {
            await Context.DisposeAsync();
            await _container.DisposeAsync();
        }
    }
}