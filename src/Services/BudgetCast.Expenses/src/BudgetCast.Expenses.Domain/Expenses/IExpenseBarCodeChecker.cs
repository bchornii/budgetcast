namespace BudgetCast.Expenses.Domain.Expenses;

public interface IExpenseBarCodeChecker
{
    Task<bool> IsBarCodeValidAsync(string barCode, CancellationToken cancellationToken);
}