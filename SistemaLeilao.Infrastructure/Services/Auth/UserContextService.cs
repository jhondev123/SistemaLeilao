using Microsoft.AspNetCore.Http;
using SistemaLeilao.Core.Application.Common.Extensions;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SistemaLeilao.Infrastructure.Services.Auth
{
    public class UserContextService(
        IHttpContextAccessor httpContextAccessor,
        IBidderRepository bidderRepository,
        IAuctioneerRepository auctioneerRepository) : IUserContextService
    {
        public Guid GetUserExternalId()
        {
            var userId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return userId != null ? Guid.Parse(userId) : Guid.Empty;
        }

        public async Task<Bidder?> GetCurrentBidderAsync()
        {
            var externalId = GetUserExternalId();
            return await bidderRepository.GetByUserExternalIdAsync(externalId);
        }

        public async Task<Auctioneer?> GetCurrentAuctioneerAsync()
        {
            var externalId = GetUserExternalId();
            return await auctioneerRepository.GetByUserExternalIdAsync(externalId);
        }

        public bool IsInRole(RoleEnum role)
        {
            return httpContextAccessor.HttpContext?.User?.IsInRole(role.GetDescription()) ?? false;
        }
    }
}
