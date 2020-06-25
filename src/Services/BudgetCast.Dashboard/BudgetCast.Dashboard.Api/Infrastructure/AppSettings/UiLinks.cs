using System.ComponentModel.DataAnnotations;

namespace BudgetCast.Dashboard.Api.Infrastructure.AppSettings
{
    public class UiLinks : Validatable
    {
        [Required, Url]
        public string Root { get; set; }

        [Required, Url]
        public string ResetPassword { get; set; }

        [Required, Url]
        public string Login { get; set; }
    }
}
