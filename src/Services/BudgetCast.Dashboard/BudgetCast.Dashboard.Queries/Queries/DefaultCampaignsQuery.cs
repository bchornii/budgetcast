using System.Collections.Generic;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Queries
{
    public class DefaultCampaignsQuery : IRequest<
        QueryResult<IReadOnlyList<string>>>
    {
        public string Term { get; set; }
        public int Amount { get; set; }
    }
}
