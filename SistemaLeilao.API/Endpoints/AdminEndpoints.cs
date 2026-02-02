using MediatR;
using SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Admin.Commands.CreateUser;

namespace SistemaLeilao.API.Endpoints
{
    public static class AdminEndpoints
    {
        private static string BaseEndpointPath = "/api/admin";

        public static void MapAdminEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(BaseEndpointPath)
                           .WithTags("Admin")
                           .RequireAuthorization(AuthorizationPolicies.AdminOnly);

            group.MapPost("/user/create", CreateUser);
        }
        private static async Task<IResult> CreateUser(CreateUserCommand command, IMediator mediator)
        {
            var result = await mediator.Send(command);
            return Results.Created($"{BaseEndpointPath}{result}", result);
        }
    }
}
