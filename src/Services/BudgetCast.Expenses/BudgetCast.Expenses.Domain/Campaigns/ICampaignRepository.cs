﻿using BudgetCast.Common.Domain;

namespace BudgetCast.Expenses.Domain.Campaigns
{
    public interface ICampaignRepository : IRepository<Campaign, long>
    {
        public Task<Campaign?> GetByNameAsync(string name);
    }
}
