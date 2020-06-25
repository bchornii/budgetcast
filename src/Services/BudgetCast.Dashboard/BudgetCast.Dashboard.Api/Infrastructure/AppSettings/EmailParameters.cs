using System.ComponentModel.DataAnnotations;

namespace BudgetCast.Dashboard.Api.Infrastructure.AppSettings
{
    public class EmailParameters : Validatable
    {
        [Required]
        public string Host { get; set; }

        [Required]
        public int Port { get; set; }

        [Required]
        public string From { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
