using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.Aggregates.Campaigns;
using BudgetCast.Dashboard.Domain.ReadModel.Campaign;
using MongoDB.Bson;
using MongoDB.Driver;


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

        private FilterDefinition<Campaign> GetFilter(string term)
        {
            return string.IsNullOrWhiteSpace(term)
                ? Builders<Campaign>.Filter.Empty
                : Builders<Campaign>.Filter.Regex("Name", 
                    BsonRegularExpression.Create(new Regex(term, RegexOptions.IgnoreCase)));
        }
    }
}
