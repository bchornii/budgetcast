using System.ComponentModel.DataAnnotations;

namespace BudgetCast.Identity.Api.Infrastructure.AppSettings
{
    public class ExternalIdentityProviders : Validatable
    {
        [Required, Url]
        public string UiRedirectUrl { get; set; }

        public IdentityProvider Google { get; set; }

        public IdentityProvider Facebook { get; set; }

        public ExternalIdentityProviders()
        {
            UiRedirectUrl = default!;
            Google = default!;
            Facebook = default!;
        }
    }

    public class IdentityProvider
    {
        [Required]
        public string Name { get; set; }

        public IdentityProvider()
        {
            Name = default!;
        }
    }
}
