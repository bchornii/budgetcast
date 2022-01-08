using BudgetCast.Common.Models;
using BudgetCast.Expenses.Domain.Expenses;
using BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses;
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

        public async Task<Expense> Add(Expense expense)
        {
            var entityEntry = await _dbContext.Expenses.AddAsync(expense);
            return entityEntry.Entity;
        }

        public async Task<Expense> GetAsync(long id)
        {
            var expense = await _dbContext.Expenses
                .Include(e => e.ExpenseItems)
                .FirstOrDefaultAsync(e => e.Id == id);

            if(expense is null)
            {
                expense = _dbContext.Expenses.Local
                    .FirstOrDefault(e => e.Id == id);
            }

            return expense;
        }

        public Task Update(Expense campaign)
        {
            _dbContext.Expenses.Update(campaign);
            return Task.CompletedTask;
        }
    }
}
