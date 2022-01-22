
namespace BudgetCast.Identity.Api.Infrastructure.Services
{
    public interface IEmailService
    {
        Task ConfirmAccount(string email, string callback);
        Task ResetPassword(string email, string callback);
    }
}