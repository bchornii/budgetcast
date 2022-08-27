namespace BudgetCast.Expenses.Domain.Expenses;

public interface IExpenseApprovalVerifier
{
    Task<bool> IsApprovedAsync(CancellationToken cancellationToken);
}