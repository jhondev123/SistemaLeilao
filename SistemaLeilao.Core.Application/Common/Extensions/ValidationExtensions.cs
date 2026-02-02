using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Common.Extensions
{
    public static class ValidationExtensions
    {
        public static IRuleBuilder<T, string> EmailStandard<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("Informe um formato de e-mail válido.")
                .MaximumLength(255).WithMessage("O e-mail deve ter no máximo 255 caracteres.");
        }
        public static IRuleBuilder<T, string> PasswordStandard<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .NotEmpty().WithMessage("A senha é obrigatória.")
                .MinimumLength(6).WithMessage("A senha deve ter pelo menos 6 caracteres.")
                .Matches(@"[A-Z]").WithMessage("A senha deve conter pelo menos uma letra maiúscula.")
                .Matches(@"[a-z]").WithMessage("A senha deve conter pelo menos uma letra minúscula.")
                .Matches(@"[0-9]").WithMessage("A senha deve conter pelo menos um número.")
                .Matches(@"[\^$*.\[\]{}()?\-""!@#%&/\,><':;|_~`]").WithMessage("A senha deve conter pelo menos um caractere especial.");

        }
    }
}
