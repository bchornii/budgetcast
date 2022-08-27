using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Commands.Expenses;

public class ExpenseBarCodeChecker : IExpenseBarCodeChecker
{
    public async Task<bool> IsBarCodeValidAsync(string barCode, CancellationToken cancellationToken)
    {
        // Mimic web api call
        await Task.Delay(3000);

        return true;
    }
}