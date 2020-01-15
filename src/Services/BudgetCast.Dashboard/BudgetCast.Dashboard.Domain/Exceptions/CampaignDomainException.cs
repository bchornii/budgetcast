using System;
using System.Runtime.Serialization;

namespace BudgetCast.Dashboard.Domain.Exceptions
{
    public class CampaignDomainException : Exception
    {
        public CampaignDomainException()
        {
        }

        protected CampaignDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public CampaignDomainException(string message) : base(message)
        {
        }

        public CampaignDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}