using System.Net;
using BudgetCast.Common.Web.ActionResults;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Common.Web.Extensions;

public static class ModelStateValidator
{
    public static IActionResult ValidateModelState(ActionContext context)
    {
        var errors = context.ModelState
            .Where(d => d.Value.Errors.Any())
            .ToDictionary(
                d => d.Key, 
                d => d.Value!.Errors.Select(e => e.ErrorMessage).ToList());
        var envelope = ProblemDetailsEnvelope.Error(errors);
        return new ProblemDetailsResult(envelope, HttpStatusCode.BadRequest);
    }
}