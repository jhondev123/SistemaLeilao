using MediatR;
using SistemaLeilao.Core.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid
{
    public record CreateBidCommand(Guid AuctionId, decimal Amount) : IRequest<Result>;
}
