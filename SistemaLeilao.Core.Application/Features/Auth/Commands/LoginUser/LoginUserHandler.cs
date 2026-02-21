using MediatR;
using SistemaLeilao.Core.Application.Common;
using SistemaLeilao.Core.Application.Interfaces;
using SistemaLeilao.Core.Domain.Common;
using SistemaLeilao.Core.Domain.Resources;

namespace SistemaLeilao.Core.Application.Features.Auth.Commands.LoginUser
{
    public class LoginUserHandler(IAuthService authService)
        : IRequestHandler<LoginUserCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            var (succeeded, token) = await authService.LoginAsync(request.Email, request.Password);

            if (!succeeded || string.IsNullOrEmpty(token))
            {
                return Result<string>.Failure(new ErrorMessage(nameof(Messages.ErrorInvalidEmailOrPassword), Messages.ErrorInvalidEmailOrPassword));
            }

            return Result<string>.Success(token, new SuccessMessage(nameof(Messages.SuccessLoginSuccessful),Messages.SuccessLoginSuccessful));
        }
    }
}