using SistemaLeilao.Core.Application.Common.Extensions;
using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public record CreateAuctionResponseDto(
    string Title,
    long AuctioneerId,
    decimal StartingPrice,
    decimal CurrentPrice,
    DateTime StartDate,
    DateTime EndDate,
    string Description,
    byte[]? Image,
    decimal MinimumIncrement,
    string Status)
    {
        public static explicit operator Auction(CreateAuctionResponseDto dto)
        {
            return new Auction(
                dto.Title,
                dto.AuctioneerId,
                dto.StartingPrice,
                dto.CurrentPrice,
                dto.StartDate,
                dto.EndDate,
                dto.Description,
                dto.Image,
                dto.MinimumIncrement,
                Domain.Enums.AuctionStatus.AWAITING_START);
        }
        public static explicit operator CreateAuctionResponseDto(Auction dto)
        {
            return new CreateAuctionResponseDto(
                dto.Title,
                dto.AuctioneerId,
                dto.StartingPrice,
                dto.CurrentPrice,
                dto.StartDate,
                dto.EndDate,
                dto.Description ?? string.Empty,
                dto.Image,
                dto.MinimumIncrement,
                Domain.Enums.AuctionStatus.AWAITING_START.GetDescription());
        }
    }

}
