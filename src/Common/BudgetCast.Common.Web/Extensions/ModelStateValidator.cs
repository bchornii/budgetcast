using System.Net;
using BudgetCast.Common.Web.ActionResults;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Common.Web.Extensions;

/// <summary>
/// Custom implementation of model state validator.
/// </summary>
public static class ModelStateValidator
{
    /// <summary>
    /// Transforms API model errors into <see cref="ProblemDetailsResult"/> result.
    /// </summary>
    public static IActionResult ValidateModelState(ActionContext context)
    {
        var errors = context.ModelState
            .Where(d => d.Value?.Errors.Any() ?? false)
            .SelectMany(d => d.Value!.Errors.Select(e => e.ErrorMessage));
        var envelope = ProblemDetailsEnvelope.Error(errors);
        return new ProblemDetailsResult(envelope, HttpStatusCode.BadRequest);
    }
}