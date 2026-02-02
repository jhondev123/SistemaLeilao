using FluentValidation;
using SistemaLeilao.Core.Application.Common.Extensions;
using SistemaLeilao.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Admin.Commands.CreateUser
{
    public  class CreateUserValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome é obrigatório.")
                .MinimumLength(3).WithMessage("O nome deve ter pelo menos 3 caracteres.")
                .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

            RuleFor(x => x.Email).EmailStandard();

            RuleFor(x => x.Password).PasswordStandard();

            RuleFor(x => x.Role)
                        .NotEmpty().WithMessage("O cargo do usuário é obrigatório.")
                        .IsEnumName(typeof(RoleEnum), caseSensitive: false)
                        .WithMessage($"O cargo deve ser um dos seguintes: {string.Join(", ", Enum.GetNames(typeof(RoleEnum)))}");
        }
    }

}
