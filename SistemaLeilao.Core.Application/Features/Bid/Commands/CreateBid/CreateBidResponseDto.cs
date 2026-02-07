using SistemaLeilao.Core.Application.Common.Extensions;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid
{
    public record CreateBidResponseDto(Guid BidId, decimal Amount)
    {
        public static CreateBidResponseDto EntityToDto(Domain.Entities.Bid bid)
        {
            return new CreateBidResponseDto(bid.ExternalId, bid.Amount);
        }
    }
}
