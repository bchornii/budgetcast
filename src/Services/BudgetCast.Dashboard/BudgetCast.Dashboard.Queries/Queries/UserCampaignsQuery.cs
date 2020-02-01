using System.Collections.Generic;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Queries
{
    public class UserCampaignsQuery : IRequest<
        QueryResult<KeyValuePair<string, string>[]>>
    {
        public string UserId { get; set; }
    }
}