using System;
using System.Collections.Generic;
using System.Linq;
using BudgetCast.Dashboard.Domain.Exceptions;
using BudgetCast.Dashboard.Domain.SeedWork;

namespace BudgetCast.Dashboard.Domain.Aggregates.Receipting
{
    public class Receipt : AggregateRoot
    {
        private const int MaxAmountOfItems = 1000;
        private const int MaxAmountOfTags = 10;
        public const string DefaultItemTitle = "Default Item";

        private DateTime _date;
        private string _description;
        private string _campaignId;

        private readonly List<ReceiptItem> _receiptItems;
        public IReadOnlyCollection<ReceiptItem> ReceiptItems => _receiptItems;

        private readonly List<string> _tags;
        public IReadOnlyCollection<string> Tags => _tags;

        protected Receipt()
        {
            _receiptItems = new List<ReceiptItem>();
            _tags = new List<string>();
        }

        public Receipt(DateTime date, string campaignId, 
            string description = null) : this()
        {
            _date = date;
            _campaignId = campaignId;
            _description = description;
        }

        public void AddItem(string title, decimal price, int quantity = 1, string description = null)
        {
            if (_receiptItems.Count > MaxAmountOfItems)
            {
                throw new ReceiptDomainException("Receipt can't hold more than 1000 items.'");
            }
            _receiptItems.Add(new ReceiptItem(title, price, quantity, description));
        }

        public void AddTags(string[] tags)
        {
            if (tags?.Length > 0)
            {
                if ((_tags.Count + tags.Length) > MaxAmountOfTags)
                {
                    throw new ReceiptDomainException(
                        "Receipt can't have more than 10 tags.");
                }
                _tags.AddRange(tags);
            }
        }

        public void SetCampaignId(string campaignId)
        {
            _campaignId = campaignId;
        }

        public decimal TotalAmount() => _receiptItems.Sum(item => item.GetPrice());
    }
}
