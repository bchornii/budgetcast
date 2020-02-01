﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Data;
using BudgetCast.Dashboard.Domain.ReadModel.General;
using BudgetCast.Dashboard.Domain.ReadModel.Receipts;
using MongoDB.Bson;
using MongoDB.Bson.IO;
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

        public async Task<PageResult<BasicReceipt>> GetBasicReceipts(string campaignId, 
            int page, int pageSize, string userId)
        {
            var total = await _context.Receipts.Collection
                .CountDocumentsAsync(new BsonDocument("CampaignId", campaignId ?? string.Empty));

            var items = await _context.ReceiptsCollection
                .AsQueryable()
                .Where(r => r.CampaignId == campaignId && r.CreatedBy == userId)
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

        public async Task<TotalsPerCampaign> GetTotals(string campaignId, string userId)
        {
            var totalAmount = await _context.ReceiptsCollection
                .AsQueryable()
                .Where(r => r.CampaignId == campaignId && r.CreatedBy == userId)
                .SelectMany(r => r.ReceiptItems)
                .SumAsync(ri => ri.Quantity * ri.Price);

            var totalsPerTags = await _context.ReceiptsCollection
                .AsQueryable()
                .Where(r => r.CampaignId == campaignId && r.CreatedBy == userId)
                .SelectMany(
                    r => r.Tags, (r, tag) => new
                    {
                        Tag = tag, 
                        RecipeTotal = r.ReceiptItems
                            .Select(ri => ri.Price * ri.Quantity).Sum()
                    })
                .GroupBy(x => x.Tag)
                .Select(g => new
                {
                    Tag = g.Key,
                    Total = g.Sum(i => i.RecipeTotal)
                })
                .ToListAsync();

            return new TotalsPerCampaign
            {
                TotalAmount = totalAmount,
                TagTotalPair = totalsPerTags.Select(r => new KeyValuePair<
                    string, decimal>(r.Tag, r.Total)).ToArray()
            };
        }
    }
}
