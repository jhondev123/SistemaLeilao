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
                .NotEmpty().WithMessage("O título do leilão é obrigatório.")
                .Length(5, 100).WithMessage("O título deve ter entre 5 e 100 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("A descrição não pode exceder 500 caracteres.")
                .MinimumLength(10).When(x => !string.IsNullOrEmpty(x.Description))
                .WithMessage("A descrição deve ser mais detalhada (mínimo 10 caracteres).");

            RuleFor(x => x.auctioneerId)
                .NotEmpty().WithMessage("O ID do leiloeiro é obrigatório.")
                .GreaterThan(0).WithMessage("ID do leiloeiro inválido.");

            RuleFor(x => x.StartingPrice)
                .GreaterThan(0).WithMessage("O preço inicial deve ser um valor positivo.");

            RuleFor(x => x.MinimumIncrement)
                .GreaterThan(0).WithMessage("O incremento mínimo deve ser maior que zero.")
                .LessThan(x => x.StartingPrice)
                .WithMessage("O incremento não deve ser maior que o preço inicial.");

            RuleFor(x => x.StartDate)
                .GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-1))
                .WithMessage("A data de início não pode ser retroativa.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("A data de término deve ser após a data de início.")
                .Must((command, endDate) => (endDate - command.StartDate).TotalHours >= 1)
                .WithMessage("O leilão deve ter uma duração mínima de pelo menos 1 hora.");

            RuleFor(x => x.Image)
                .Must(img => img == null || img.Length <= 2 * 1024 * 1024)
                .WithMessage("A imagem do leilão não pode exceder 2MB.");
        }
    }
}
