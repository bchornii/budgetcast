using System.Linq;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.ReadModel.General;
using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace BudgetCast.Dashboard.ReadAccessors
{
    public class ReceiptReadAccessor : IReceiptReadAccessor
    {
        private readonly BudgetCastContext _context;

        public ReceiptReadAccessor(BudgetCastContext context)
        {
            _context = context;
        }

        public async Task<PageResult<BasicReceipt>> GetBasicReceipts(
            string campaignId, int page, int pageSize)
        {
            var total = await _context.Receipts.Collection
                .CountDocumentsAsync(new BsonDocument("CampaignId", campaignId ?? string.Empty));

            var items = await _context.ReceiptsCollection
                .AsQueryable()
                .Where(r => r.CampaignId == campaignId)
                .OrderByDescending(r => r.Date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new BasicReceipt
                {
                    Id = r.Id,
                    Tags = r.Tags,
                    TotalItems = r.ReceiptItems.Length,
                    TotalAmount = r.ReceiptItems.Sum(ri => ri.Price),
                    Date = r.Date,
                    Description = r.Description,
                    CreatedBy = r.CreatedBy
                })
                .ToListAsync();
                
            return new PageResult<BasicReceipt>(items, total);
        }
    }
}
