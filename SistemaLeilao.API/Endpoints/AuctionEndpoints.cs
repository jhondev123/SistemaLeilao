using MediatR;
using SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction;

namespace SistemaLeilao.API.Endpoints
{
    public static class AuctionEndpoints
    {
        private static string BaseEndpointPath = "/api/auctions";
        public static void MapAuctionEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(BaseEndpointPath)
                           .WithTags("Auctions")
                           .RequireAuthorization();

            group.MapPost("/", CreateAuction);
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
