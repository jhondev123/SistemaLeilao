using MediatR;
using SistemaLeilao.Core.Application.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Admin.Commands.CreateUser
{

    public record CreateUserCommand(string Name, string Email, string Password, string Role): IRequest<Result>;
}
