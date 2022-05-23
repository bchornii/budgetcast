using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BudgetCast.Common.Application.Behavior.Validation
{
    /// <summary>
    /// For any command of type <see cref="ICommand{TResult}"/> or query of type <see cref="IQuery{TResult}"/>
    /// where <c>TResult</c> is <see cref="Result"/> or <seealso cref="Result{T}"/> runs list of validators
    /// to determine if it's valid.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
#pragma warning disable CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint.
    public class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TResponse : Result
#pragma warning restore CS8714 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'notnull' constraint
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;

        public ValidatorBehavior(
            IEnumerable<IValidator<TRequest>> validators,
            ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var commandName = request!.GetGenericTypeName();
            _logger.LogInformation("Validating {CommandName}", commandName);

            var validationResults = await Task
                .WhenAll(_validators.Select(v => v.ValidateAsync(request, cancellationToken)));

            var validationFailures = validationResults
                .SelectMany(result => result.Errors)
                .Where(error => error is not null)
                .ToArray();

            if (validationFailures.Any())
            {
                _logger.LogWarning("Validation of {CommandName} failed", commandName);

                var mostSevereErrorCode = validationFailures.GetMostSevereErrorCode();
                var errors = validationFailures.GetErrors();

                if (typeof(TResponse).IsGenericResult())
                {
                    var genericArgumentType = typeof(TResponse)
                        .GetGenericResultArgumentType();

                    var genericResult = mostSevereErrorCode.OrGeneralErrorCodeIfNull()
                        .AsGenericResultOf(genericArgumentType, errors);

                    return (genericResult as TResponse)!;
                }

                var result = mostSevereErrorCode.OrGeneralErrorCodeIfNull()
                    .AsResult(errors);

                return (result as TResponse)!;
            }

            _logger.LogInformation("Validation of {CommandName} passed", commandName);

            return await next();
        }
    }
}
