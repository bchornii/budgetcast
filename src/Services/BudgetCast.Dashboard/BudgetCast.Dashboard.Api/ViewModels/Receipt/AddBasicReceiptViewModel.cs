using System;

namespace BudgetCast.Dashboard.Api.ViewModels.Receipt
{
    public class AddBasicReceiptViewModel
    {
        public DateTime Date { get; set; }
        public string[] Tags { get; set; }
        public string Campaign { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
