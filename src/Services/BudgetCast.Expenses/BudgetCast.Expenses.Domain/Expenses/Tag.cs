using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public record Tag : ValueObject
    {
        public string Name { get; init; }
    }
}
