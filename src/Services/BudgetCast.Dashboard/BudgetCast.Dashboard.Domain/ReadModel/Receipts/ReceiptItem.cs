using BudgetCast.Dashboard.Domain.SeedWork;

namespace BudgetCast.Dashboard.Domain.ReadModel.Receipts
{
    public class ReceiptItem : IdentifiableReadModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
