using BudgetCast.Common.Domain;
using BudgetCast.Expenses.Domain.Expenses;

namespace BudgetCast.Expenses.Domain;

public static class Errors
{
    public const string AppPrefix = "app";

    public static class Campaigns
    {
        public static readonly string ErrorsCampaigns = $"{AppPrefix}.{nameof(Campaigns)}".ToLowerInvariant();

        public static Error CampaignNameCantBeEmpty()
            => new(ErrorsCampaigns, "Campaign name cannot be empty.");

        public static Error CampaignStartDateShouldBeLessThanCompletionDate()
            => new(ErrorsCampaigns, "Campaign start date should not be ahead of complete date.");

        public static Error CampaignWithTheSameNameAlreadyExists()
            => new(ErrorsCampaigns, "Campaign with the same name already exists.");
    }
    
    public static class Expenses
    {
        public static readonly string ErrorsExpenses = $"{AppPrefix}.{nameof(Expenses)}".ToLowerInvariant();
        
        public static Error NotMoreThan30Tags() 
            => new(ErrorsExpenses, "Receipt can't have more than 10 tags.");

        public static Error NoMoreThan1000Items() => new(ErrorsExpenses, "Receipt can't hold more than 1000 items.");

        public static Error AddedAtIsLessThan365() 
            => new(ErrorsExpenses, "Expense can be created with past date not more than 365 ago.");

        public static class Tags
        {
            public static readonly string ErrorsTags = $"{AppPrefix}.{nameof(Tag)}".ToLowerInvariant();

            public static Error TagNameShouldHaveValue()
                => new(ErrorsTags, "Tag name should have non empty name.");
        }

        public static class ExpenseItems
        {
            public static readonly string ErrorsExpenseItem = $"{AppPrefix}.{nameof(ExpenseItem)}".ToLowerInvariant();
            
            public static Error ExpenseItemPriceIsZero()
                => new(ErrorsExpenseItem, "Expense item price should be greater that 0.");

            public static Error ItemsQuantityIsMoreThan1000()
                => new(ErrorsExpenseItem, "Expense item quantity should be between 1 and 1000.");

            public static Error ItemShouldHaveTitle()
                => new(ErrorsExpenseItem, "Expense item should have title set.");

            public static Error NoteShouldHaveText()
                => new(ErrorsExpenseItem, "Note should contain some text.");
        }
    }
}