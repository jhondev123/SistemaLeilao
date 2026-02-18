using SistemaLeilao.Tests.Common.Builders;
using SistemaLeilao.Tests.Common.Fixtures;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.IntegrationTests.Features.Bid
{
    [Collection("Database")]
    public class BidPlacedConsumerTests : IAsyncLifetime
    {
        private readonly DatabaseFixture _fixture;

        public BidPlacedConsumerTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
        }


        public async ValueTask InitializeAsync() => await _fixture.ResetAsync();
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;

        [Fact]
        public async Task Consume_LanceValido_DevePersistirEAtualizarPreco()
        {
            // banco sempre limpo aqui
            var auction = new AuctionBuilder().Build();
            _fixture.Context.Auctions.Add(auction);
            await _fixture.Context.SaveChangesAsync();

            // ...
        }
    }
}
