using FluentValidation;

namespace BudgetCast.Expenses.Queries.Expenses.SearchForExistingTagsByName
{
    public class SearchForExistingTagsByNameQueryValidator : 
        AbstractValidator<SearchForExistingTagsByNameQuery>
    {
        public SearchForExistingTagsByNameQueryValidator()
        {
            RuleFor(x => x.Term)
                .NotEmpty();

            RuleFor(x => x.Amount)
                .InclusiveBetween(1, 100);
        }
    }
}
