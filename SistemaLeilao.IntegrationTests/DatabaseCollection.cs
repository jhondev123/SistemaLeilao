using SistemaLeilao.IntegrationTests.Fixtures;
using Xunit;

namespace SistemaLeilao.IntegrationTests
{
    [CollectionDefinition("DatabaseCollection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}