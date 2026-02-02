using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SistemaLeilao.Core.Application.Common
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (!_validators.Any()) return await next();

            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

            if (failures.Any())
            {
                var errors = failures.Select(f => f.ErrorMessage).Distinct();

                return CreateFailureResponse(errors);
            }

            return await next();
        }

        private TResponse CreateFailureResponse(IEnumerable<string> errors)
        {
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                return (TResponse)typeof(TResponse)
                    .GetMethod("Failure", [typeof(IEnumerable<string>)])?
                    .Invoke(null, [errors])!;
            }

            return (TResponse)(object)Result.Failure(errors);
        }
    }
}
