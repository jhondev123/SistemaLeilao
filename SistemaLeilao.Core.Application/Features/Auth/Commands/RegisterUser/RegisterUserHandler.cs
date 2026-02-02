using MediatR;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auth.Commands.RegisterUser
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result>
    {
        private readonly IAuthService authService;
        public RegisterUserHandler(IAuthService authService)
        {
            this.authService = authService;
        }
        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var (succeeded, errors) = await authService.RegisterAsync(request.Name, request.Email, request.Password, request.WantToBeAuctioneer);

            if (!succeeded)
                return Result.Failure(errors);

            return Result.Success("Usuário criado com sucesso!");
        }
    }
}
