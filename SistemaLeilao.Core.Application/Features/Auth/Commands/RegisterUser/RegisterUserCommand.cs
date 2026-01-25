using MediatR;
using SistemaLeilao.Core.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auth.Commands.RegisterUser
{
    public record RegisterUserCommand(string Name,string Email, string Password)
    : IRequest<Result>;
}
