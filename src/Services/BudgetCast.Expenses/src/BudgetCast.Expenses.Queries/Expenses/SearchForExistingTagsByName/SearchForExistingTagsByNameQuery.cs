﻿using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;

namespace BudgetCast.Expenses.Queries.Expenses.SearchForExistingTagsByName
{
    public record SearchForExistingTagsByNameQuery(int Amount, string TagTerm) : 
        IQuery<Result<IReadOnlyList<string>>>;

    public class SearchForExistingTagsByNameQueryHandler : 
        IQueryHandler<SearchForExistingTagsByNameQuery, Result<IReadOnlyList<string>>>
    {
        private readonly IExpensesDataAccess _expensesDataAccess;

        public SearchForExistingTagsByNameQueryHandler(IExpensesDataAccess expensesDataAccess)
        {
            _expensesDataAccess = expensesDataAccess;
        }

        public async Task<Result<IReadOnlyList<string>>> Handle(
            SearchForExistingTagsByNameQuery request, 
            CancellationToken cancellationToken)
        {
            var results = await _expensesDataAccess
                .SearchForTagsAsync(request.TagTerm, request.Amount);
            return new Success<IReadOnlyList<string>>(results);
        }
    }
}
