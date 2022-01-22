using BudgetCast.Identity.Api.Infrastructure.AppSettings;
using MailKit.Net.Smtp;
using MimeKit;

namespace BudgetCast.Identity.Api.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailParameters _emailParameters;

        public EmailService(EmailParameters emailParameters)
        {
            _emailParameters = emailParameters;
        }

        public Task ResetPassword(string email, string callback)
        {
            return SendEmailAsync(email, "Reset Password",
                "In order to reset your password please follow " +
               $"this <a href='{callback}'>link</a>.");
        }

        public Task ConfirmAccount(string email, string callback)
        {
            return SendEmailAsync(email, "Account Confirmation",
                "In order to confirm your account registration please " +
               $"follow this <a href='{callback}'>link</a>.");
        }

        private async Task SendEmailAsync(string email, string subject, string callback)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Budget Cast", _emailParameters.From));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = callback
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
