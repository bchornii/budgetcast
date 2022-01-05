using BudgetCast.Expenses.Domain.Campaigns;

namespace BudgetCast.Expenses.Data.Campaigns
{
    public class CampaignRepository : ICampaignRepository
    {
        private readonly ExpensesDbContext _dbContext;

        public CampaignRepository(ExpensesDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Campaign> Add(Campaign campaign)
        {
            var entityEntry = await _dbContext.Campaigns.AddAsync(campaign);
            return entityEntry.Entity;
        }

        public async Task<Campaign> GetAsync(ulong id) 
            => await _dbContext.Campaigns.FindAsync(id);

        public Task Update(Campaign campaign)
        {
            _dbContext.Campaigns.Update(campaign);
            return Task.CompletedTask;
        }
    }
}
