using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
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
                .Find(new BsonDocument("Name", name))
                .Project(c => c.Id).FirstOrDefaultAsync();
        }
    }
}
