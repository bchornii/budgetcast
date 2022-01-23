namespace BudgetCast.Identity.Api.Infrastructure.AppSettings
{
    public class JwtSettings
    {
        public string Issuer { get; set; }

        public string Audience { get; set; }

        public string? Key { get; set; }

        public int TokenExpirationInMinutes { get; set; }

        public int RefreshTokenExpirationInDays { get; set; }

        public JwtSettings()
        {
            Issuer = default!;
            Audience = default!;
        }
    }
}
