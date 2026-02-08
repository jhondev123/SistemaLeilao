using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Interfaces
{
    public interface IUserContextService
    {
        Guid GetUserExternalId();
        Task<Bidder?> GetCurrentBidderAsync();
        Task<Auctioneer?> GetCurrentAuctioneerAsync();
        bool IsInRole(RoleEnum role);
    }
}
