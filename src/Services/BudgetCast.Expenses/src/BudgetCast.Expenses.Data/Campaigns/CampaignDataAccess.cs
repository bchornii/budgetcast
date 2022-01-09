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

        /// <summary>
        /// Retrieves campaigns by matching them via name in SQL-like way.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="campaignName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<CampaignVm>> GetAsync(
            int amount, 
            string campaignName, 
            CancellationToken cancellationToken)
        {
            var query = _context.Campaigns
                .AsNoTracking()
                .Where(x => EF.Functions.Like(x.Name, $"{campaignName}%"));

            if(amount == 1)
            {
                var result = await query
                    .FirstOrDefaultAsync(cancellationToken: cancellationToken);

                if(result is null)
                {
                    return Array.Empty<CampaignVm>();
                }

                return new[]
                {
                    new CampaignVm
                    {
                        Id = result.Id,
                        Name = result.Name,
                    }
                };
            }
            else
            {
                return await query
                    .Select(x => new CampaignVm
                    {
                        Id = x.Id,
                        Name = x.Name,
                    })
                    .ToListAsync();
            }
        }

        /// <summary>
        /// Retrieves first campaign that matches passed name in a SQL-like way.
        /// </summary>
        /// <param name="campaignName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<CampaignVm> GetAsync(string campaignName, CancellationToken cancellationToken)
            => (await GetAsync(amount: 1, campaignName, cancellationToken))[0];

        /// <summary>
        /// Returns all tenant's campaigns
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<IReadOnlyList<CampaignVm>> GetAsync(CancellationToken cancellationToken)
        {
            return await _context.Campaigns
                .AsNoTracking()
                .Select(c => new CampaignVm
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToArrayAsync(cancellationToken: cancellationToken);
        }
    }
}
