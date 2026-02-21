using MediatR;
using SistemaLeilao.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auth.Commands.LoginUser
{
    public record LoginUserCommand(string Email, string Password)
    : IRequest<Result<string>>;

}
