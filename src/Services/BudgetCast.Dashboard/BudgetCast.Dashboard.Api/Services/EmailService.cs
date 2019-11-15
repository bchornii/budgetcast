using BudgetCast.Dashboard.Api.AppSettings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

namespace BudgetCast.Dashboard.Api.Services
{
    public class EmailService
    {
        private readonly EmailParameters _emailParameters;

        public EmailService(IOptions<EmailParameters> options)
        {
            _emailParameters = options.Value;
        }

        public async Task SendEmailAsync(string email, string callback)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Budget Cast", _emailParameters.From));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = "Account Confirmation";
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = "Please confirm your account registration " +
                      $"by this <a href='{callback}'>link</a>."
            };

            using (var client = new SmtpClient())
            {
                await client.ConnectAsync(_emailParameters.Host, _emailParameters.Port, false);
                await client.AuthenticateAsync(_emailParameters.From, _emailParameters.Password);
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}
