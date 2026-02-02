using FluentValidation;
using SistemaLeilao.API.Core.Application.Features.Auctions.Commands.CreateAuction;
using System;
using System.Collections.Generic;
using System.Text;
namespace SistemaLeilao.Core.Application.Features.Auctions.Commands.CreateAuction
{
    public class CreateAuctionValidator : AbstractValidator<CreateAuctionCommand>
    {
        public CreateAuctionValidator()
        {
            RuleFor(x => x.Title)
            .NotEmpty().WithMessage("O título do leilão não pode ser vazio.")
            .Length(5, 100).WithMessage("O título deve ter entre 5 e 100 caracteres.");

            RuleFor(x => x.InitialPrice)
                .GreaterThan(0).WithMessage("O preço inicial deve ser um valor positivo.");

            RuleFor(x => x.StartsAt)
                .GreaterThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("A data de início não pode ser no passado.");

            RuleFor(x => x.EndsAt)
                .GreaterThan(x => x.StartsAt)
                .WithMessage("O leilão deve terminar após a data de início.");
        }
    }
}
