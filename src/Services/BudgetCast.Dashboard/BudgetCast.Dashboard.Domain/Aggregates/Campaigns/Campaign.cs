using System;
using BudgetCast.Dashboard.Domain.Exceptions;
using BudgetCast.Dashboard.Domain.SeedWork;

namespace BudgetCast.Dashboard.Domain.Aggregates.Campaigns
{
    public class Campaign : AggregateRoot
    {
        private string _name;
        private DateTime _startsAt;
        private DateTime _completesAt;

        protected Campaign() { }

        public Campaign(string name) : this()
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new CampaignDomainException("Campaign should have title.");
            }
            _name = name;
            (_startsAt, _completesAt) = GetFirstAndLastDaysOfTheMonth();
        }

        public Campaign(string title, DateTime startsAt, DateTime completesAt) : this(title)
        {
            if (startsAt < completesAt)
            {
                throw new CampaignDomainException("Campaign start date should be ahead of complete date.");
            }
            _startsAt = startsAt;
            _completesAt = completesAt;
        }

        private (DateTime, DateTime) GetFirstAndLastDaysOfTheMonth()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return (firstDayOfMonth, lastDayOfMonth);
        }
    }
}
