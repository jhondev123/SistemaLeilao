using MediatR;
using SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction;
using SistemaLeilao.Core.Application.Features.Auth.Commands.LoginUser;
using SistemaLeilao.Core.Application.Features.Auth.Commands.RegisterUser;

namespace SistemaLeilao.API.Endpoints
{
    public static class AuthEndpoints
    {
        private static string BaseEndpointPath = "/api/auth";

        public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
        {
            var group = app.MapGroup(BaseEndpointPath)
                           .WithTags("Auth");

            group.MapPost("/register", RegisterUser);
            group.MapPost("/login", LoginUser);
        }
        private static async Task<IResult> RegisterUser(RegisterUserCommand command, IMediator mediator)
        {
            var result = await mediator.Send(command);
            return result.IsSuccess
                    ? Results.Created("/api/auth/login", result)
                    : Results.BadRequest(result);
        }
        private static async Task<IResult> LoginUser(LoginUserCommand command, IMediator mediator)
        {
            var result = await mediator.Send(command);

            if (!result.IsSuccess)
            {
                return Results.Json(result, statusCode: StatusCodes.Status401Unauthorized);
            }

            return Results.Ok(result);
        }
    }
}
