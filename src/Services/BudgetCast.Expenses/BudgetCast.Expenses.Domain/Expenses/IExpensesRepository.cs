using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public interface IExpensesRepository : IRepository<Expense, long>
    {
        Task TestMeOut();
    }
}
