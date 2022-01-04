using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public record Tag(string Name, string ExpenseId) : ValueObject;
}
