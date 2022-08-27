using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Expenses;

public class ExpenseApprovalVerifier : IExpenseApprovalVerifier
{
    public async Task<bool> IsApprovedAsync(CancellationToken cancellationToken)
    {
        // Mimic web api call
        await Task.Delay(3000);

        return true;
    }
}