using System;
using System.Runtime.Serialization;

namespace BudgetCast.Dashboard.Domain.Exceptions
{
    public class ReceiptDomainException : Exception
    {
        public ReceiptDomainException()
        {
        }

        protected ReceiptDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public ReceiptDomainException(string message) : base(message)
        {
        }

        public ReceiptDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
