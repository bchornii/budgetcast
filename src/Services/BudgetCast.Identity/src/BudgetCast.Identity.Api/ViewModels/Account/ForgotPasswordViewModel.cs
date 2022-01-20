namespace BudgetCast.Identity.Api.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        public string Email { get; set; }

        public ForgotPasswordViewModel()
        {
            Email = default!;
        }
    }
}
