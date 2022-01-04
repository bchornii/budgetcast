namespace BudgetCast.Common.Application.Behavior.Idempotency
{
    /// <summary>
    /// The registry to track if an operation with the same CorrelationId was processed before
    /// </summary>
    public interface IOperationsRegistry
    {
        /// <summary>
        /// Tries to add the current operation to the registry.
        /// If it is already there - returns false and the result of previous execution
        /// </summary>
        /// <returns></returns>
        Task<(bool IsOperationExists, string OperationResult)> TryAddCurrentOperationAsync();

        /// <summary>
        /// Marks the operation as completed and save the result if needed
        /// </summary>
        /// <returns></returns>
        Task SetCurrentOperationCompletedAsync();

        /// <summary>
        /// Marks the operation as completed and save the result if needed
        /// </summary>
        /// <returns></returns>
        Task SetCurrentOperationCompletedAsync(string result);
    }
}