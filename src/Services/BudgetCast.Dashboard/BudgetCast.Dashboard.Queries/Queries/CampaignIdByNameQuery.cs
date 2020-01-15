using BudgetCast.Dashboard.Queries.Results;
using MediatR;

namespace BudgetCast.Dashboard.Queries.Queries
{
    public class CampaignIdByNameQuery : IRequest<QueryResult<string>>
    {
        public string CampaignName { get; set; }
    }
}
