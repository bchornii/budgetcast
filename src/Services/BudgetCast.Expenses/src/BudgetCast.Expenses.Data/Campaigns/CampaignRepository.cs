using BudgetCast.Expenses.Domain.Campaigns;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Expenses.Data.Campaigns
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly ExpensesDbContext _dbContext;

        public CampaignRepository(ExpensesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Campaign> AddAsync(Campaign campaign, CancellationToken cancellationToken)
        {
            var entityEntry = await _dbContext.Campaigns.AddAsync(campaign, cancellationToken);
            return entityEntry.Entity;
        }

        public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken)
        {
            return await _dbContext.Campaigns.AnyAsync(c => c.Name == name, cancellationToken: cancellationToken);
        }

        public async Task<Campaign> GetAsync(long id, CancellationToken cancellationToken)
        {
            var campaign = await _dbContext.Campaigns
                .FirstAsync(c => c.Id == id, cancellationToken: cancellationToken);

            if(campaign is null)
            {
                campaign = _dbContext.Campaigns.Local
                    .FirstOrDefault(e => e.Id == id);
            }

            return campaign;
        }

        public Task<Campaign?> GetByNameAsync(string name, CancellationToken cancellationToken) 
            => _dbContext.Campaigns.FirstOrDefaultAsync(c => c.Name == name, cancellationToken: cancellationToken);

        public Task UpdateAsync(Campaign campaign, CancellationToken cancellationToken)
        {
            _dbContext.Campaigns.Update(campaign);
            return Task.CompletedTask;
        }
    }
}
