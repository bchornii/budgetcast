using System.Runtime.Serialization;

namespace BudgetCast.Common.Domain.Results;

public class ResultValueIsNullException : Exception
{
    public ResultValueIsNullException()
    {
    }

    protected ResultValueIsNullException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public ResultValueIsNullException(string? message) : base(message)
    {
    }

    public ResultValueIsNullException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}