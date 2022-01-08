using BudgetCast.Expenses.Queries.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Data.Campaigns
{
    public class CampaignDataAccess : ICampaignDataAccess
    {
        private readonly ExpensesDbContext _context;

        public CampaignDataAccess(ExpensesDbContext context)
        {
            _context = context;
        }

        public async Task<CampaignVm> GetAsync(string campaignName, CancellationToken cancellationToken)
        {
            var result = await _context.Campaigns
                .AsNoTracking()
                .FirstAsync(x => x.Name == campaignName, cancellationToken: cancellationToken);

            return new CampaignVm
            {
                Id = result.Id,
                Name = result.Name,
            };
        }
    }
}
