using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Common.Web.ActionResults;

public class ProblemDetailsEnvelope : ProblemDetails
{
    /// <summary>
    /// Represents collection of errors for model properties. Key represents a property name
    /// for which value in a form of array of error messages was generated.
    /// </summary>
    public IDictionary<string, List<string>>? Errors { get; }
        
    /// <summary>
    /// Response timestamp.
    /// </summary>
    public DateTime GeneratedAt { get; }

    private ProblemDetailsEnvelope(IDictionary<string, List<string>> errors)
    {
        Errors = errors;
        GeneratedAt = DateTime.UtcNow;

        Title = "Validation error";
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        Detail = $"See '{nameof(Errors).ToLowerInvariant()}' collection for details";
        Instance = nameof(ProblemDetailsEnvelope);
    }

    /// <summary>
    /// Creates error result based on <paramref name="errors"/> passed in.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static ProblemDetailsEnvelope Error(IDictionary<string, List<string>> errors) => new(errors);
}