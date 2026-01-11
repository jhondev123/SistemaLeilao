using MediatR;
using SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public class CreateAuctionHandler : IRequestHandler<CreateAuctionCommand, Guid>
    {
        public async Task<Guid> Handle(CreateAuctionCommand request, CancellationToken cancellationToken)
        {
            // 1. Validar lógica de negócio
            // 2. Criar entidade de Domínio
            // 3. Persistir (via Repository)

            return Guid.NewGuid(); // Mock por enquanto
        }
    }
}
