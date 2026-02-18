using MediatR;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Application.Common;

namespace SistemaLeilao.API.Endpoints
{
    public static class AuctionEndpoints
    {
        private static string BaseEndpointPath = "/api/auctions";
        public static void MapAuctionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(BaseEndpointPath)
                           .WithTags("Auctions");

            group.MapPost("/", CreateAuction).RequireAuthorization(AuthorizationPolicies.AuctioneerOnly);
            //group.MapGet("/{id}", GetAuctionById);
        }
        private static async Task<IResult> CreateAuction(CreateAuctionCommand command, IMediator mediator)
        {
            var result = await mediator.Send(command);
            return Results.Created($"{BaseEndpointPath}{result}", result);
        }
        //private static async Task<IResult> GetAuctionById(CreateAuctionCommand command, IMediator mediator)
        //{
        //    var result = await mediator.Send(command);
        //    return Results.Created($"/api/auctions/{result.Id}", result);
        //}
    }
}
