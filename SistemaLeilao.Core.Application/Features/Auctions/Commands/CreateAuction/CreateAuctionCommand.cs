using MediatR;

namespace SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public record CreateAuctionCommand(string Title, decimal InitialPrice,DateTime StartsAt, DateTime EndsAt)
        : IRequest<Guid>;
}
