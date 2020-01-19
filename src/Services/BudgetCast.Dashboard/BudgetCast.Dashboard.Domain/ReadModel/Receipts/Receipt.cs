using System;
using BudgetCast.Dashboard.Domain.SeedWork;

namespace BudgetCast.Dashboard.Domain.ReadModel.Receipts
{
    public class Receipt : HistoricalReadModel
    {
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string CampaignId { get; set; }
        public string[] Tags { get; set; }
        public ReceiptItem[] ReceiptItems { get; set; }
    }
}