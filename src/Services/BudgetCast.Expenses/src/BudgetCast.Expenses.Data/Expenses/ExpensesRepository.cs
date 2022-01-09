using BudgetCast.Expenses.Domain.Expenses;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Data.Expenses
{
    public class ExpensesRepository : IExpensesRepository
    {
        private readonly ExpensesDbContext _dbContext;

        public ExpensesRepository(ExpensesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Expense> AddAsync(Expense expense, CancellationToken cancellationToken)
        {
            var entityEntry = await _dbContext.Expenses.AddAsync(expense, cancellationToken);
            return entityEntry.Entity;
        }

        public async Task<Expense> GetAsync(long id, CancellationToken cancellationToken)
        {
            var expense = await _dbContext.Expenses
                .Include(e => e.ExpenseItems)
                .FirstOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken);

            if(expense is null)
            {
                expense = _dbContext.Expenses.Local
                    .FirstOrDefault(e => e.Id == id);
            }

            return expense;
        }

        public Task UpdateAsync(Expense campaign, CancellationToken cancellationToken)
        {
            _dbContext.Expenses.Update(campaign);
            return Task.CompletedTask;
        }
    }
}
