using MediatR;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Domain.Entities;

namespace SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public record CreateAuctionCommand(
        string Title, 
        long auctioneerId,
        decimal StartingPrice, 
        DateTime StartDate, 
        DateTime EndDate,
        string? Description,
        byte[]? Image,
        decimal MinimumIncrement
        )
        : IRequest<Result<Auction?>>;
}
