using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Events
{
    public record BidPlacedEvent(Guid AuctionId, Guid BidderId, decimal Amount);

}
