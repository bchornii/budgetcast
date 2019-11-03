namespace BudgetCast.Dashboard.Api.AppSettings
{
    public class ExternalIdentityProviders
    {
        public string UiRedirectUrl { get; set; }
        public IdentityProvider Google { get; set; }
    }

    public class IdentityProvider
    {
        public string Name { get; set; }
    }
}
