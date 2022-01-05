using BudgetCast.Expenses.Domain.Expenses;

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

        public async Task<Expense> GetAsync(ulong id) 
            => await _dbContext.Expenses.FindAsync(id);

        public Task Update(Expense campaign)
        {
            _dbContext.Expenses.Update(campaign);
            return Task.CompletedTask;
        }
    }
}
