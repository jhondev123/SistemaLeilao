using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Interfaces;

namespace SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid
{
    public class CreateBidHandler(
        IBidRepository bidRepository,
        IAuctionRepository auctionRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateBidCommand, bool>
    {
        public async Task<bool> Handle(CreateBidCommand request, CancellationToken ct)
        {
            Domain.Entities.Auction? auction = await auctionRepository.GetByIdAsync(request.AuctionId);
            if(auction is null)
            {
                throw new KeyNotFoundException("Auction not found.");
            }

            Domain.Entities.Bid newBid = new(request.Amount, request.BidderId, request.AuctionId);
            auction.UpdatePrice(request.Amount, request.BidderId);

            bidRepository.Add(newBid);
            auctionRepository.Update(auction);

            var result = await unitOfWork.CommitAsync(ct);

            return result > 0;
        }
    }
}
