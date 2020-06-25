using System.ComponentModel.DataAnnotations;

namespace BudgetCast.Dashboard.Api.Infrastructure.AppSettings
{
    public class ExternalIdentityProviders : Validatable
    {
        [Required, Url]
        public string UiRedirectUrl { get; set; }

        public IdentityProvider Google { get; set; }
        public IdentityProvider Facebook { get; set; }
    }

    public class IdentityProvider
    {
        [Required]
        public string Name { get; set; }
    }
}
