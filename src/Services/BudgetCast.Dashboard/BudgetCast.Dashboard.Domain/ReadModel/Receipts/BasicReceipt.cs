using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetCast.Dashboard.Domain.ReadModel.Receipts
{
    public class BasicReceipt
    {
        public string CreatedBy { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public int? TotalItems { get; set; }
        public decimal? TotalAmount { get; set; }
        public string[] Tags { get; set; }
    }
}
