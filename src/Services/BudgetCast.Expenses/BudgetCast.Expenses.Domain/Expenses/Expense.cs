using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Domain.Campaigns;

namespace BudgetCast.Expenses.Domain.Expenses
{
    public class Expense : AggregateRoot
    {
        private long _campaignId;
        private long _campaignTenantId;

        public DateTime AddedAt { get; private set; }

        public string Description { get; private set; }

        public decimal TotalPrice { get; private set; }

        private readonly List<ExpenseItem> _expenseItems;
        public IReadOnlyCollection<ExpenseItem> ExpenseItems => _expenseItems;

        private readonly List<Tag> _tags;
        public IReadOnlyCollection<Tag> Tags => _tags;

        protected Expense()
        {
            Description = default!;
            _expenseItems = new List<ExpenseItem>();
            _tags = new List<Tag>();
        }

        public Expense(
            DateTime addedAt,
            Campaign campaign, 
            string description = "no description") : this()
        {
            AddedAt = addedAt;
            Description = description;

            _campaignId = campaign.Id;
            _campaignTenantId = campaign.TenantId;
        }

        public virtual void AddItem(ExpenseItem expenseItem)
        {
            if (_expenseItems.Count >= 100)
            {
                throw new Exception("Receipt can't hold more than 1000 items.'");
            }

            _expenseItems.Add(expenseItem);
            RecalculateTotalPrice();
        }

        public virtual void AddTags(Tag[] tags)
        {
            if ((_tags.Count + tags.Length) > 100)
            {
                throw new Exception("Receipt can't have more than 10 tags.");
            }

            var nonExistingTags = tags
                .Where(t => !_tags.Contains(t)).ToArray();
            _tags.AddRange(nonExistingTags);
        }

        public virtual void SetCampaignId(long campaignId)
        {
            _campaignId = campaignId;
        }

        public virtual decimal GetTotalAmount() 
            => _expenseItems.Sum(item => item.GetTotalPrice());

        private void RecalculateTotalPrice()
        {
            TotalPrice = GetTotalAmount();
        }
    }
}
