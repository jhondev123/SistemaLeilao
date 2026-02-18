using MediatR;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Domain.Entities;

namespace SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public record CreateAuctionCommand(
        string Title, 
        decimal StartingPrice, 
        DateTime StartDate, 
        DateTime EndDate,
        string? Description,
        byte[]? Image,
        decimal MinimumIncrement
        )
        : IRequest<Result<CreateAuctionResponseDto?>>;
}
