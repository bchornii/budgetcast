using BudgetCast.Common.Application.Command;
using BudgetCast.Common.Extensions;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Operations;

namespace BudgetCast.Common.Application.Behavior.Idempotency;

/// <summary>
/// For any command of type <see cref="ICommand{TResult}"/> where <c>TResult</c> is
/// <see cref="Result"/> or <seealso cref="Result{T}"/> verifies if it was previously executed by
/// relying on <see cref="IOperationsRegistry"/> as a source.
/// </summary>
public class IdempotentBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
    where TResponse : Result
{
    private readonly IOperationsRegistry _operationsRegistry;
    private readonly ILogger<IdempotentBehavior<TRequest, TResponse>> _logger;

    public IdempotentBehavior(
        IOperationsRegistry operationsRegistry,
        ILogger<IdempotentBehavior<TRequest, TResponse>> logger)
    {
        _operationsRegistry = operationsRegistry;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        var commandName = typeof(TRequest).GetGenericTypeName();

        _logger.LogInformation("Checking {CommandName} for prior execution", commandName);
        var (isOperationExists, operationResult) = await _operationsRegistry.TryAddCurrentOperationAsync(cancellationToken);

        if (isOperationExists)
        {
            _logger.LogInformation("Operation {CommandName} has been already executed and won't be repeated", commandName);

            if (!string.IsNullOrWhiteSpace(operationResult))
            {
                var data = GetGenericResultOf(operationResult);
                return (data as TResponse)!;
            }

            return (Success.Empty as TResponse)!;
        }

        _logger.LogInformation("Sending {CommandName} command for execution", commandName);
        var result = await next();
        _logger.LogInformation("{CommandName} command executed", commandName);

        var (isOfSuccessType, isGeneric) = result.CheckIfSuccess();

        if (isOfSuccessType)
        {
            if (isGeneric)
            {
                var json = JsonSerializer.Serialize(result, result.GetType(), AppConstants.DefaultOptions);
                _logger.LogInformation("Saving operation result of {CommandName} with payload {Payload}", commandName, json);

                await _operationsRegistry.SetCurrentOperationCompletedAsync(json, cancellationToken);
            }
            else
            {
                _logger.LogInformation("Saving operation result of {CommandName} with empty payload", commandName);
                await _operationsRegistry.SetCurrentOperationCompletedAsync((CancellationToken) cancellationToken);
            }

            _logger.LogInformation("Operation result of {CommandName} saved", commandName);
        }

        return result;
    }

    private static object GetGenericResultOf(string operationResult)
    {
        var genericArgumentType = typeof(TResponse)
            .GetGenericResultArgumentType();

        var genericResultType = typeof(Success<>)
            .MakeGenericType(genericArgumentType);

        var data = JsonSerializer.Deserialize(
            json: operationResult,
            returnType: genericResultType,
            options: AppConstants.DefaultOptions);

        return data!;
    }
}