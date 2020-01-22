using System;

namespace BudgetCast.Dashboard.Domain.ReadModel.Receipts
{
    public class BasicReceipt
    {
        public string Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int? TotalItems { get; set; }
        public decimal? TotalAmount { get; set; }
        public string[] Tags { get; set; }
    }
}
