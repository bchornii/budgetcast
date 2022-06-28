using System.Runtime.Serialization;

namespace BudgetCast.Common.Domain.Results.Exceptions;

public class ResultValueIsNullException : Exception
{
    public IDictionary<string, List<string>> Errors { get; } = default!;
    
    public ResultValueIsNullException()
    {
    }

    protected ResultValueIsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ResultValueIsNullException(string? message) : base(message)
    {
    }
    
    public ResultValueIsNullException(IDictionary<string, List<string>> errors)
    {
        Errors = errors;
    }

    public ResultValueIsNullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}