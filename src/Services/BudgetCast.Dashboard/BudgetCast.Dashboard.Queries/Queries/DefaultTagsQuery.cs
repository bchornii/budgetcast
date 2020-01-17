using System.Collections.Generic;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Queries
{
    public class DefaultTagsQuery : IRequest<
        QueryResult<IReadOnlyList<string>>>
    {
        public string UserId { get; set; }
        public string Term { get; set; }
        public int Amount { get; set; }
    }
}
