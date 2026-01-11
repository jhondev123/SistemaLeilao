using MediatR;

namespace SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public record CreateAuctionCommand(string Title, decimal InitialPrice, DateTime EndsAt)
        : IRequest<Guid>;
}
