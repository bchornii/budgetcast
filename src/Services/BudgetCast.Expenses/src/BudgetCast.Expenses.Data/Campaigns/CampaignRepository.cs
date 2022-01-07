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

        public async Task<Campaign> Add(Campaign campaign)
        {
            var entityEntry = await _dbContext.Campaigns.AddAsync(campaign);
            return entityEntry.Entity;
        }

        public async Task<Campaign> GetAsync(long id) 
            => await _dbContext.Campaigns.FindAsync(id);

        public Task<Campaign?> GetByNameAsync(string name) 
            => _dbContext.Campaigns.FirstOrDefaultAsync(c => c.Name == name);

        public Task Update(Campaign campaign)
        {
            _dbContext.Campaigns.Update(campaign);
            return Task.CompletedTask;
        }
    }
}
