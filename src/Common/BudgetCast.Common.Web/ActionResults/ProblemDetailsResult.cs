using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Common.Web.ActionResults;

/// <summary>
/// Represents custom API result based on <see cref="ProblemDetailsEnvelope"/>.
/// </summary>
public class ProblemDetailsResult : IActionResult
{
    private readonly ProblemDetailsEnvelope _envelope;
    private readonly HttpStatusCode _httpStatusCode;

    public ProblemDetailsResult(ProblemDetailsEnvelope envelope, HttpStatusCode httpStatusCode)
    {
        _envelope = envelope;
        _httpStatusCode = httpStatusCode;
    }
        
    public Task ExecuteResultAsync(ActionContext context)
    {
        var objectResult = new ObjectResult(_envelope)
        {
            StatusCode = (int)_httpStatusCode,
        };
        return objectResult.ExecuteResultAsync(context);
    }
}