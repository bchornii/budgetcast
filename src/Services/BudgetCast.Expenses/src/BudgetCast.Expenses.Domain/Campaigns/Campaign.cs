using BudgetCast.Common.Domain;
using BudgetCast.Common.Domain.Results;

namespace BudgetCast.Expenses.Domain.Campaigns
{
    public class Campaign : AggregateRoot
    {
        public string Name { get; private set; }

        public DateTime StartsAt { get; private set; }

        public DateTime? CompletesAt { get; private set; }

        private Campaign() 
        {
            Name = default!;
        }

        private Campaign(string name) : this()
        {
            Name = name;
            (StartsAt, CompletesAt) = GetFirstAndLastDaysOfTheMonth();
        }

        public static Result<Campaign> Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return InvariantViolations.Campaigns.CampaignNameCantBeEmpty();
            }

            return new Campaign(name);
        }

        public static Result<Campaign> Create(string name, DateTime startsAt, DateTime? completesAt)
        {
            if (startsAt > completesAt)
            {
                return InvariantViolations.Campaigns.CampaignStartDateShouldBeLessThanCompletionDate();
            }

            var campaign = Create(name).Value;
            campaign.StartsAt = startsAt;
            campaign.CompletesAt = completesAt;

            return campaign;
        }

        private static (DateTime, DateTime) GetFirstAndLastDaysOfTheMonth()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return (firstDayOfMonth, lastDayOfMonth);
        }
    }
}
