using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Features.Bid.Commands.CreateBid
{
    public class CreateBidValidator : AbstractValidator<CreateBidCommand>
    {
        public CreateBidValidator()
        {
            RuleFor(x => x.AuctionId)
                .NotEmpty().WithMessage("O ID do leilão é obrigatório.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("O valor do lance deve ser maior que zero.")
                .Must(HaveValidDecimals).WithMessage("O valor do lance não pode ter mais de duas casas decimais.");
        }
        private bool HaveValidDecimals(decimal amount)
        {
            return decimal.Round(amount, 2) == amount;
        }
    }
}
