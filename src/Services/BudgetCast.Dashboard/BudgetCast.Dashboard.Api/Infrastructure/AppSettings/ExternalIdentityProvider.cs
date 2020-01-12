namespace BudgetCast.Dashboard.Api.Infrastructure.AppSettings
{
    public class ExternalIdentityProviders
    {
        public string UiRedirectUrl { get; set; }
        public IdentityProvider Google { get; set; }
        public IdentityProvider Facebook { get; set; }
    }

    public class IdentityProvider
    {
        public string Name { get; set; }
    }
}
