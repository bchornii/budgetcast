namespace BudgetCast.Common.Operations;

public sealed class NoStorageOperationsRegistry : IOperationsRegistry
{
    public Task<(bool IsOperationExists, string OperationResult)> TryAddCurrentOperationAsync(
        CancellationToken cancellationToken) 
        => Task.FromResult((false, string.Empty));

    public Task SetCurrentOperationCompletedAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    public Task SetCurrentOperationCompletedAsync(string result, CancellationToken cancellationToken) => Task.CompletedTask;
}