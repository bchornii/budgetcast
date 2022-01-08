using BudgetCast.Common.Models;
using BudgetCast.Expenses.Queries.Expenses;
using BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Data.Expenses
{
    public class ExpensesDataAccess : IExpensesDataAccess
    {
        private readonly ExpensesDbContext _context;

        public ExpensesDataAccess(ExpensesDbContext context)
        {
            _context = context;
        }

        public async Task<PageResult<ExpenseVm>> GetAsync(
            long campaignId, 
            int pageNumber, 
            int pageSize, 
            CancellationToken cancellationToken)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 10 : pageSize;

            var query = _context.Expenses
                .Include(e => e.ExpenseItems)
                .Where(e => EF.Property<long>(e, "_campaignId") == campaignId)
                .OrderByDescending(e => e.Id);

            var count = await query
                .IgnoreQueryFilters()
                .AsNoTracking()
                .CountAsync(cancellationToken: cancellationToken);

            var items = await query
                .IgnoreQueryFilters()
                .AsNoTracking()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ExpenseVm
                {
                    Id = x.Id,
                    AddedAt = x.AddedAt,
                    CreatedBy = x.CreatedBy,
                    Description = x.Description,
                    Tags = x.Tags.Select(x => x.Name).ToArray(),
                    TotalAmount = x.TotalPrice,
                    TotalItems = x.ExpenseItems.Count
                })
                .ToListAsync(cancellationToken: cancellationToken);

            return new PageResult<ExpenseVm>(items, pageSize, pageNumber, count);
        }
    }
}
