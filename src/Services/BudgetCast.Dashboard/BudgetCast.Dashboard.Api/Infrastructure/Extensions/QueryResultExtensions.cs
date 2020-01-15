using BudgetCast.Dashboard.Queries.Results;
using Microsoft.AspNetCore.Mvc;
using static BudgetCast.Dashboard.Queries.Results.ResultStatus;

namespace BudgetCast.Dashboard.Api.Infrastructure.Extensions
{
    public static class QueryResultExtensions
    {
        public static IActionResult ToHttpActionResult<T>(this QueryResult<T> queryResult)
        {
            if (queryResult == null)
            {
                return new NoContentResult();
            }

            switch (queryResult.Status)
            {
                case Success:
                    return new OkObjectResult(queryResult.Value);
                case BadQuery:
                    return new BadRequestObjectResult(queryResult.Messages);
                case NotFound:
                    return new NotFoundObjectResult(queryResult.Messages);
                default:
                    return new NoContentResult();
            }
        }
    }
}