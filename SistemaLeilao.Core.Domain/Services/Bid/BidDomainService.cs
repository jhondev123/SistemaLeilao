using SistemaLeilao.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Domain.Services.Bid
{
    public class BidDomainService
    {
        public (bool success, string error) ValidateBid(
            Auction? auction,
            Bidder? bidder,
            decimal amount)
        {
            if (auction is null)
                return (false, "Leilão não encontrado.");

            if (bidder is null)
                return (false, "Licitante não encontrado.");

            if (auction.AuctioneerId == bidder.Id)
                return (false, "O leiloeiro não pode dar lance no próprio leilão.");

            return auction.CanPlaceBid(amount);
        }
    }
}
