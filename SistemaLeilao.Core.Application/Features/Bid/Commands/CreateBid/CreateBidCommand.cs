using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid
{
    public record CreateBidCommand(long AuctionId,long BidderId, decimal Amount) : IRequest<bool>;
}
