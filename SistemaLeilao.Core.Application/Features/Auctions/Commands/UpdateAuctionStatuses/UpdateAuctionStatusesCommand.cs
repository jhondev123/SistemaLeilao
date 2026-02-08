using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auctions.Commands.UpdateAuctionStatuses
{
    public record UpdateAuctionStatusesCommand : IRequest;
}
