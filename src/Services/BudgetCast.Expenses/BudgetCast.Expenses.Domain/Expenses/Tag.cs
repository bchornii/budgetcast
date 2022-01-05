using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public class Tag
    {
        public string Name { get; set; }

        public ulong ExpenseId { get; set; }
    }
}
