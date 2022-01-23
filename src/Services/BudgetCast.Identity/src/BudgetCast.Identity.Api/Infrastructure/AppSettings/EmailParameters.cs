using System.ComponentModel.DataAnnotations;

namespace BudgetCast.Identity.Api.Infrastructure.AppSettings
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

        public EmailParameters()
        {
            Host = default!;
            Port = default!;
            From = default!;
            Password = default!;
        }
    }
}
