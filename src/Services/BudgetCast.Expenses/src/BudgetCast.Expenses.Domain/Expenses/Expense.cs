using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;
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

        private Expense()
        {
            Description = default!;
            _expenseItems = new List<ExpenseItem>();
            _tags = new List<Tag>();
        }

        private Expense(
            DateTime addedAt,
            Campaign campaign, 
            string description) : this()
        {
            AddedAt = addedAt;
            Description = description ?? "no description";

            _campaignId = campaign.Id;
            _campaignTenantId = campaign.TenantId;
        }

        public static Result<Expense> Create(
            DateTime addedAt,
            Campaign campaign,
            string description)
        {
            var validatedAddedAt = IsValidAddingDt(addedAt).Value;
            return new Expense(validatedAddedAt, campaign, description);
        }

        public static Result<DateTime> IsValidAddingDt(DateTime value)
        {
            if (value < DateTime.Now.AddDays(-365))
            {
                return Errors.Expenses.AddedAtIsLessThan365();
            }
            
            return value;
        }

        public virtual Result AddItem(ExpenseItem expenseItem)
        {
            if (_expenseItems.Count >= 100)
            {
                return Errors.Expenses.NoMoreThan1000Items();
            }

            _expenseItems.Add(expenseItem);
            RecalculateTotalPrice();
            
            return Success.Empty;
        }

        public virtual Result AddTags(Tag[] tags)
        {
            if ((_tags.Count + tags.Length) > 100)
            {
                return Errors.Expenses.NotMoreThan30Tags();
            }

            var nonExistingTags = tags
                .Where(t => !_tags.Contains(t)).ToArray();
            _tags.AddRange(nonExistingTags);
            
            return Success.Empty;
        }

        public virtual Result SetCampaignId(long campaignId)
        {
            _campaignId = campaignId;
            return Success.Empty;
        }

        public long GetCampaignId() => _campaignId;

        public virtual decimal GetTotalAmount() 
            => _expenseItems.Sum(item => item.GetTotalPrice());

        private void RecalculateTotalPrice()
        {
            TotalPrice = GetTotalAmount();
        }
    }
}
