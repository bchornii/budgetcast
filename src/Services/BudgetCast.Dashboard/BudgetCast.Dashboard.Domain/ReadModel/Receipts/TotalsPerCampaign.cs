using System.Collections.Generic;

namespace BudgetCast.Dashboard.Domain.ReadModel.Receipts
{
    public class TotalsPerCampaign
    {
        public decimal TotalAmount { get; set; }
        public KeyValuePair<string, decimal>[] TagTotalPair { get; set; }
    }
}