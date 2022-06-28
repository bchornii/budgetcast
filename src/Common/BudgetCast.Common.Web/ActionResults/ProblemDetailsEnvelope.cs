using Microsoft.AspNetCore.Mvc;

namespace BudgetCast.Common.Web.ActionResults;

public class ProblemDetailsEnvelope : ProblemDetails
{
    public const string NotClassifiedErrorsKey = "api.model";

    /// <summary>
    /// Represents collection of errors for model properties. Key represents a property name
    /// for which value in a form of array of error messages was generated.
    /// </summary>
    public IDictionary<string, List<string>> Errors { get; }

    /// <summary>
    /// Response timestamp.
    /// </summary>
    public DateTime GeneratedAt { get; }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
        Justification = "Too conform with problem details use lowercase")]
    private ProblemDetailsEnvelope(IDictionary<string, List<string>> errors)
    {
        Errors = errors;
        GeneratedAt = DateTime.UtcNow;
        Title = "Application Validation Error";
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        Detail = $"See '{nameof(Errors).ToLowerInvariant()}' collection for details";
        Instance = nameof(ProblemDetailsEnvelope);
    }

    private ProblemDetailsEnvelope(IDictionary<string, List<string>> errors, IDictionary<string, object> extensions)
    {
        Errors = errors;
        GeneratedAt = DateTime.UtcNow;
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        foreach (var extension in extensions)
        {
            Extensions.Add(extension);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase",
        Justification = "Too conform with problem details use lowercase")]
    private ProblemDetailsEnvelope(IEnumerable<string> errors)
    {
        Errors = new Dictionary<string, List<string>> {[NotClassifiedErrorsKey] = errors.ToList(),};
        GeneratedAt = DateTime.UtcNow;
        Title = "API model Validation Error";
        Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1";
        Detail = $"See '{nameof(Errors).ToLowerInvariant()}' collection for details";
        Instance = nameof(ProblemDetailsEnvelope);
    }

    /// <summary>
    /// Creates error envelope based on <paramref name="errors"/> passed in.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static ProblemDetailsEnvelope Error(IDictionary<string, List<string>> errors) => new(errors);

    /// <summary>
    /// Creates error envelope based on <paramref name="errors"/> and <paramref name="extensions"/>.
    /// </summary>
    /// <param name="errors"></param>
    /// <param name="extensions"></param>
    /// <returns></returns>
    public static ProblemDetailsEnvelope Exception(
        IDictionary<string, List<string>> errors,
        IDictionary<string, object> extensions)
    {
        var envelope = new ProblemDetailsEnvelope(errors, extensions)
        {
            Title = "Application Exception",
            Instance = nameof(ProblemDetailsEnvelope),
        };

        return envelope;
    }

    /// <summary>
    /// Creates error envelope based on <paramref name="errors"/> passed in under <see cref="NotClassifiedErrorsKey"/> key.
    /// </summary>
    /// <param name="errors"></param>
    /// <returns></returns>
    public static ProblemDetailsEnvelope Error(IEnumerable<string> errors) => new(errors);
}