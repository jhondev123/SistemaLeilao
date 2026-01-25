using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Common
{
    public static class AuthorizationPolicies
    {
        public const string AdminOnly = "AdminOnly";
        public const string AuctioneerOnly = "AuctioneerOnly";
        public const string BidderOnly = "ClientOnly";
    }
}
