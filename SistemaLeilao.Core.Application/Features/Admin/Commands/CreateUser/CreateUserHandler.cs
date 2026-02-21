using MediatR;
using Microsoft.Extensions.Logging;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Common.Extensions;
using SistemaLeilao.Core.Application.Features.Auth.Commands.LoginUser;
using SistemaLeilao.Core.Application.Features.Auth.Commands.RegisterUser;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Entities;
using SistemaLeilao.Core.Domain.Enums;
using SistemaLeilao.Core.Domain.Interfaces;
using SistemaLeilao.Core.Domain.Interfaces.Repositories;
using SistemaLeilao.Core.Domain.Resources;
using System;
using System.Collections.Generic;
using System.Text;
namespace SistemaLeilao.Core.Application.Features.Admin.Commands.CreateUser
{

    public class CreateUserHandler(
        IUnitOfWork unitOfWork,
        IAuthService _authService, 
        IAuctioneerRepository _auctioneerRepository,
        IBidderRepository _bidderRepository,
        ILogger<CreateUserHandler> logger
    ) : IRequestHandler<CreateUserCommand, Result>
    {

        public async Task<Result> Handle(CreateUserCommand request, CancellationToken ct)
        {
            logger.LogInformation("Iniciando criação de usuário com email: {Email}", request.Email);
            var (succeeded, errors, userId) = await _authService.CreateUserAsync(request.Name, request.Email, request.Password, request.Role);
            if (!succeeded)
            {
                logger.LogWarning("Falha ao criar usuário com email: {Email}. Erros: {Errors}", request.Email, string.Join(", ", errors));
                return Result.Failure(errors);
            }

            if (request.Role == RoleEnum.Auctioneer.GetDescription())
            {
                logger.LogInformation("Criando entidade Auctioneer para o usuário com ID: {UserId}", userId);

                var auctioneer = new Auctioneer(request.Name, request.Email, userId!.Value);
                _auctioneerRepository.Add(auctioneer);
            }

            if(request.Role == RoleEnum.Bidder.GetDescription())
            {
                logger.LogInformation("Criando entidade Bidder para o usuário com ID: {UserId}", userId);
                var bidder = new Bidder(request.Name, userId!.Value);
                _bidderRepository.Add(bidder);

            }

            var result = await unitOfWork.CommitAsync(ct);

            if(result > 0)
            {
                logger.LogInformation("Usuário com email: {Email} criado com sucesso.", request.Email);
                return Result.Success(new SucessMessage(nameof(Messages.SucessUserCreated),Messages.SucessUserCreated));
            }
            logger.LogError("Falha ao salvar alterações no banco de dados para o usuário com email: {Email}", request.Email);
            return Result.Failure(new ErrorMessage(nameof(Messages.ErrorFailToCreateUser),Messages.ErrorFailToCreateUser));
        }
    }
}