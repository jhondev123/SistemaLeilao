using MediatR;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Features.Auth.Commands.LoginUser;
using SistemaLeilao.Core.Application.Features.Auth.Commands.RegisterUser;
using SistemaLeilao.Core.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Admin.Commands.CreateUser
{

    public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result>
    {
        private readonly IAuthService authService;
        public CreateUserHandler(IAuthService authService)
        {
            this.authService = authService;
        }

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var (succeeded, errors) = await authService.CreateUserAsync(request.Name, request.Email, request.Password, request.Role);
            if (!succeeded)
                return Result.Failure(errors);
            return Result.Success("Usuário criado com sucesso!");
        }
    }
}