using FluentValidation;
using SistemaLeilao.Core.Application.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserValidator : AbstractValidator<LoginUserCommand>
    {
        public LoginUserValidator()
        {
            RuleFor(x => x.Email).EmailStandard();

            RuleFor(x => x.Password).PasswordStandard();
        }
    }
}
