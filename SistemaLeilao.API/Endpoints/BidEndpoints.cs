using MediatR;
using SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid;

namespace SistemaLeilao.API.Endpoints
{
    public static class BidEndpoints
    {
        private static string BaseEndpointPath = "/api/bids";

        public static void MapAuctionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(BaseEndpointPath)
                           .WithTags("Bids");

            group.MapPost("/", CreateBid)
                .RequireAuthorization(AuthorizationPolicies.BidderOnly);
        }
        private static async Task<IResult> CreateBid(CreateBidCommand command, IMediator mediator)
        {
            var result = await mediator.Send(command);
            return Results.Created($"{BaseEndpointPath}{result}", result);
        }
    }
}
