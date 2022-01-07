using BudgetCast.Common.Application;
using BudgetCast.Common.Application.Queries;
using BudgetCast.Expenses.Queries.Data;
using Dapper;

namespace BudgetCast.Expenses.Queries.Campaigns.GetCampaignByName
{
    public record GetCampaignByNameQuery : IQuery<Result<CampaignViewModel>>
    {
        public string Name { get; init; }

        public GetCampaignByNameQuery()
        {
            Name = default!;
        }
    }

    public class GetCampaignByNameQueryHandler : IQueryHandler<GetCampaignByNameQuery, Result<CampaignViewModel>>
    {
        private readonly ISqlConnectionFactory _connectionFactory;

        public GetCampaignByNameQueryHandler(ISqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<Result<CampaignViewModel>> Handle(
            GetCampaignByNameQuery request, 
            CancellationToken cancellationToken)
        {
            using var connection = await _connectionFactory
                .GetOpenConnection(cancellationToken);

            var result = await connection
                .QuerySingleAsync<CampaignViewModel>(
                    "SELECT c.Id, c.Name " +
                    "FROM dbo.Campaigns as c " +
                    "WHERE c.Name = @Name", 
                    new { request.Name });

            return new Success<CampaignViewModel>(result);
        }
    }
}
