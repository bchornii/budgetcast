using System.ComponentModel.DataAnnotations;

namespace BudgetCast.Identity.Api.Infrastructure.AppSettings
{
    public class UiLinks : Validatable
    {
        [Required, Url]
        public string Root { get; set; }

        [Required, Url]
        public string ResetPassword { get; set; }

        [Required, Url]
        public string Login { get; set; }

        public UiLinks()
        {
            Root = default!;
            ResetPassword = default!;
            Login = default!;
        }
    }
}
