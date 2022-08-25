using System.Net;
using BudgetCast.Common.Domain.Results;
using BudgetCast.Common.Web.ActionResults;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Common.Web.Extensions
{
    public static class ActionResultExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            return result switch
            {
                Success => new OkResult(),
                NotFound notFound => new ProblemDetailsResult(ProblemDetailsEnvelope.Error(notFound.Errors), HttpStatusCode.NotFound),
                InvalidInput invalidInput => new ProblemDetailsResult(ProblemDetailsEnvelope.Error(invalidInput.Errors), HttpStatusCode.BadRequest),
                Forbidden forbidden => new ProblemDetailsResult(ProblemDetailsEnvelope.Error(forbidden.Errors), HttpStatusCode.Forbidden),
                GeneralFail generalFail => new ProblemDetailsResult(ProblemDetailsEnvelope.Error(generalFail.Errors), HttpStatusCode.BadRequest),
                _ => new OkResult(),
            };
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result switch
            {
                Success<T> success => new OkObjectResult(success.Value),
                NotFound<T> notFound => new ProblemDetailsResult(ProblemDetailsEnvelope.Error(notFound.Errors), HttpStatusCode.NotFound),
                InvalidInput<T> invalidInput => new ProblemDetailsResult(ProblemDetailsEnvelope.Error(invalidInput.Errors), HttpStatusCode.BadRequest),
                Forbidden<T> forbidden => new ProblemDetailsResult(ProblemDetailsEnvelope.Error(forbidden.Errors), HttpStatusCode.Forbidden),
                GeneralFail<T> generalFail => new ProblemDetailsResult(ProblemDetailsEnvelope.Error(generalFail.Errors), HttpStatusCode.BadRequest),
                _ => new OkResult(),
            };
        }
    }
}
