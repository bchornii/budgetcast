using IdentityModel;
using Microsoft.AspNetCore.Authentication;

namespace BudgetCast.Notifications.AppHub.Infrastructure.Extensions
{
    public static class AuthenticationTicketExtensions
    {
        public static DateTime GetExpiration(this AuthenticationTicket ticket)
        {
            var exp = ticket?.Principal?.Claims?
                .FirstOrDefault(c => c.Type == JwtClaimTypes.Expiration);

            if (exp == null)
            {
                return DateTime.MinValue;
            }

            return long.TryParse(exp.Value, out var r)
                ? DateTimeOffset.FromUnixTimeSeconds(r).UtcDateTime
                : DateTime.MinValue;
        }
    }
}
