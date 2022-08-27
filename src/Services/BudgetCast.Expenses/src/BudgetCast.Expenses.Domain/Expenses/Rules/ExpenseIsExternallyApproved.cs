using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Expenses.Domain.Expenses.Rules;

public class ExpenseIsExternallyApproved : AbstractBusinessRule
{
    private readonly IExpenseApprovalVerifier _expenseApprovalVerifier;

    public ExpenseIsExternallyApproved(IExpenseApprovalVerifier expenseApprovalVerifier) 
        => _expenseApprovalVerifier = expenseApprovalVerifier;

    public override async Task<Result> ValidateAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(300, cancellationToken);
        return Success.Empty;
    }
}