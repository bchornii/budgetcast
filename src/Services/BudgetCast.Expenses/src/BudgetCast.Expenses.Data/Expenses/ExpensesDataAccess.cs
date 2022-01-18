using BudgetCast.Common.Models;
using BudgetCast.Common.Web.Middleware;
using BudgetCast.Expenses.Queries.Campaigns.GetCampaignTotals;
using BudgetCast.Expenses.Queries.Expenses;
using BudgetCast.Expenses.Queries.Expenses.GetExpenseById;
using BudgetCast.Expenses.Queries.Expenses.GetExpensesForCampaign;
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
                @"SELECT Name
                  FROM dbo.Tags
                  WHERE Name LIKE @tagTerm 
                    AND ExpenseTenantId = @tenantId",
                param: new
                {
                    tagTerm = $"{tagTerm}%",
                    tenantId = TenantId,
                });

            return result.Distinct().OrderBy(x => x).Take(amount).ToArray();
        }

        public async Task<ExpenseDetailsVm> GetAsync(long expenseId, CancellationToken cancellationToken)
        {
            return await _context.Expenses
                .Include(e => e.ExpenseItems)
                .Where(e => e.Id == expenseId)
                .Select(e => new ExpenseDetailsVm
                {
                    Id = e.Id,
                    CampaignId = EF.Property<long>(e, "_campaignId"),
                    AddedBy = e.CreatedBy,
                    CreatedOn = e.CreatedOn,
                    AddedAt = e.AddedAt,
                    Description = e.Description,
                    Tags = e.Tags.Select(t => t.Name).ToArray(),
                    ExpenseItems = e.ExpenseItems.Select(ei => new ExpenseItemDetailsVm
                    {
                        Id = ei.Id,
                        Note = ei.Note ?? string.Empty,
                        Price = ei.Price,
                        Quantity = ei.Quantity,
                        Title = ei.Title
                    }).ToArray(),
                })
                .FirstAsync(cancellationToken: cancellationToken);
        }

        public async Task<TotalsPerCampaignVm> GetTotalsAsync(string campaignName, CancellationToken cancellationToken)
        {
            var tagsWithExpenses = await   
                        (
                            from e in _context.Expenses
                            join c in _context.Campaigns
                                on new
                                {
                                    CampaignTenantId = EF.Property<long>(e, "_campaignTenantId"),
                                    CampaignId = EF.Property<long>(e, "_campaignId")
                                }
                                equals new
                                {
                                    CampaignTenantId = c.TenantId,
                                    CampaignId = c.Id
                                }
                            where c.Name == campaignName
                            select e
                        )
                        .SelectMany(e => e.Tags, (e, tag) => new
                        {
                            ExpensePrice = new
                            {
                                e.Id,
                                Value = e.TotalPrice,
                            },
                            tag.Name,
                        })
                .ToArrayAsync(cancellationToken: cancellationToken);

            var total = tagsWithExpenses
                .GroupBy(x => x.ExpensePrice.Id)
                .Select(g => g.First().ExpensePrice.Value)
                .Sum();

            var totalsPerTags = tagsWithExpenses
                .GroupBy(t => t.Name)
                .Select(g => new KeyValuePair<string, decimal>(
                    key: g.Key,
                    value: g.Sum(x => x.ExpensePrice.Value)))
                .ToArray();

            return new TotalsPerCampaignVm
            {
                TotalAmount = total,
                TagTotalPair = totalsPerTags,
            };
        }
    }
}
