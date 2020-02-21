using BudgetCast.Dashboard.Commands.Results;
using Microsoft.AspNetCore.Mvc;
using static BudgetCast.Dashboard.Commands.Results.ResultStatus;

namespace BudgetCast.Dashboard.Api.Infrastructure.Extensions
{
    public static class CommandResultExtensions
    {
        public static IActionResult ToHttpActionResult(this CommandResult commandResult)
        {
            if (commandResult == null)
            {
                return new NoContentResult();
            }

            switch (commandResult.Status)
            {
                case Success:
                    return new OkResult();
                case Failed:
                    return new BadRequestObjectResult(commandResult.Errors);
                case ValidationViolation:
                    return new BadRequestObjectResult(commandResult.Errors);
                default:
                    return new NoContentResult();
            }
        }

        public static IActionResult ToHttpActionResult<T>(this CommandResult<T> commandResult)
        {
            if (commandResult == null)
            {
                return new NoContentResult();
            }

            switch (commandResult.Status)
            {
                case Success:
                    return new OkObjectResult(commandResult.Result);
                case Failed:
                    return new BadRequestObjectResult(commandResult.Errors);
                case ValidationViolation:
                    return new BadRequestObjectResult(commandResult.Errors);
                default:
                    return new NoContentResult();
            }
        }
    }
}