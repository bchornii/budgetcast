using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.ReadModel.Campaigns;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Campaign = BudgetCast.Dashboard.Domain.Aggregates.Campaigns.Campaign;


namespace BudgetCast.Dashboard.ReadAccessors
{
    public class CampaignReadAccessor : ICampaignReadAccessor
    {
        private readonly BudgetCastContext _context;

        public CampaignReadAccessor(BudgetCastContext context)
        {
            _context = context;
        }

        public async Task<string> GetIdByName(string name)
        {
            return await _context.Campaigns.Collection
                .Find(new BsonDocument("Name", name?.Trim() ?? string.Empty))
                .Project(c => c.Id).FirstOrDefaultAsync();
        }

        public async Task<string[]> GetCampaigns(string term, int amount)
        {
            var filter = GetFilter(term);
            return (await _context.Campaigns.Collection
                .Find(filter)
                .Limit(amount)
                .Sort(new BsonDocument("Name", 1))
                .Project<CampaignName>("{Name : 1, _id: 0}")
                .ToListAsync()).Select(t => t.Name).ToArray();
        }

        public async Task<KeyValuePair<string, string>[]> GetCampaigns(string userId)
        {
            return (await _context.CampaignsCollection
                .AsQueryable()
                .Where(c => c.CreatedBy == userId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new { c.Id, c.Name })
                .ToListAsync()).Select(r => new 
                    KeyValuePair<string, string>(r.Id, r.Name)).ToArray();
        }

        private FilterDefinition<Campaign> GetFilter(string term)
        {
            return string.IsNullOrWhiteSpace(term)
                ? Builders<Campaign>.Filter.Empty
                : Builders<Campaign>.Filter.Regex("Name", 
                    BsonRegularExpression.Create(new Regex(term, RegexOptions.IgnoreCase)));
        }
    }
}
