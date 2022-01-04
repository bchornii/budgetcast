using BudgetCast.Common.Application;
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
                NotFound notFound => new NotFoundObjectResult(new
                {
                    notFound.Errors,
                }),
                InvalidInput invalidInput => new BadRequestObjectResult(new
                {
                    invalidInput.Errors,
                }),
                GeneralFail generalFail => new BadRequestObjectResult(new
                {
                    generalFail.Errors,
                }),
                _ => new OkResult(),
            };
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            return result switch
            {
                Success<T> success => new OkObjectResult(success.Data),
                NotFound<T> notFound => new NotFoundObjectResult(new
                {
                    notFound.Errors,
                }),
                InvalidInput<T> invalidInput => new BadRequestObjectResult(new
                {
                    invalidInput.Errors,
                }),
                GeneralFail<T> generalFail => new BadRequestObjectResult(new
                {
                    generalFail.Errors,
                }),
                _ => new OkResult(),
            };
        }
    }
}
