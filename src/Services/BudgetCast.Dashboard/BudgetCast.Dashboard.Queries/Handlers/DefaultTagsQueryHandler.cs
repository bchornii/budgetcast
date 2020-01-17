using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BudgetCast.Dashboard.Domain.ReadModel.Tags;
using BudgetCast.Dashboard.Queries.Queries;
using BudgetCast.Dashboard.Queries.Results;
using MediatR;
using static BudgetCast.Dashboard.Queries.Results.QueryResult<
    System.Collections.Generic.IReadOnlyList<string>>;

namespace BudgetCast.Dashboard.Queries.Handlers
{
    public class DefaultTagsQueryHandler : IRequestHandler<
        DefaultTagsQuery, QueryResult<IReadOnlyList<string>>>
    {
        private readonly IDefaultTagReadAccessor _tagReadAccessor;

        public DefaultTagsQueryHandler(IDefaultTagReadAccessor tagReadAccessor)
        {
            _tagReadAccessor = tagReadAccessor;
        }

        public async Task<QueryResult<IReadOnlyList<string>>> Handle(
            DefaultTagsQuery request, CancellationToken cancellationToken)
        {
            var tags = await _tagReadAccessor.GetTags(
                request.UserId, request.Term, request.Amount);

            return GetSuccessResult(tags);
        }
    }
}
