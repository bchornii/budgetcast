using System.Runtime.Serialization;

namespace BudgetCast.Common.Domain.Results;

public class AddingErrorsToSuccessResultException : Exception
{
    public AddingErrorsToSuccessResultException()
    {
    }

    protected AddingErrorsToSuccessResultException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    public AddingErrorsToSuccessResultException(string? message) : base(message)
    {
    }

    public AddingErrorsToSuccessResultException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}