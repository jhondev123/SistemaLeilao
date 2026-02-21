using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Services.Bid
{
    public class BidDomainService
    {
        public (bool success, ErrorMessage error) ValidateBid(
            Auction? auction,
            Bidder? bidder,
            decimal amount)
        {
            if (auction is null)
                return (false, new ErrorMessage(nameof(Messages.ErrorAuctionNotFound), Messages.ErrorAuctionNotFound));

            if (bidder is null)
                return (false, new ErrorMessage(nameof(Messages.ErrorBidderNotFound), Messages.ErrorBidderNotFound));

            if (auction.AuctioneerId == bidder.Id)
                return (false, new ErrorMessage(nameof(Messages.ErrorAuctioneerDoNotBidInOwnAuction), Messages.ErrorAuctioneerDoNotBidInOwnAuction));

            return auction.CanPlaceBid(amount);
        }
    }
}
