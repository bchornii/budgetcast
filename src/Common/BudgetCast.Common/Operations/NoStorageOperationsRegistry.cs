namespace BudgetCast.Common.Operations;

public sealed class NoStorageOperationsRegistry : IOperationsRegistry
{
    public Task<(bool IsOperationExists, string OperationResult)> TryAddCurrentOperationAsync() 
        => Task.FromResult((false, string.Empty));

    public Task SetCurrentOperationCompletedAsync() => Task.CompletedTask;

    public Task SetCurrentOperationCompletedAsync(string result) => Task.CompletedTask;
}