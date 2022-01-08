using BudgetCast.Common.Models;
using BudgetCast.Common.Web.Middleware;
using BudgetCast.Expenses.Queries.Expenses;
using BudgetCast.Expenses.Queries.Expenses.GetCampaingExpenses;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Data.Expenses
{
    public class ExpensesDataAccess : IExpensesDataAccess
    {
        private readonly ExpensesDbContext _context;

        public long TenantId { get; }

        public ExpensesDataAccess(ExpensesDbContext context, ITenantService tenantService)
        {
            _context = context;
            TenantId = tenantService.TenantId;
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

        public async Task<IReadOnlyList<string>> SearchForTagsAsync(string tagTerm, int amount)
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<string>(
                $"SELECT TOP {amount} Name " +
                "FROM dbo.Tags " +
                "WHERE Name LIKE @tagTerm AND ExpenseTenantId = @tenantId",
                param: new
                {
                    tagTerm = $"{tagTerm}%",
                    tenantId = TenantId,
                });

            return result.Distinct().OrderBy(x => x).ToArray();
        }
    }
}
