namespace BudgetCast.Identity.Api.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string Code { get; set; }

        public ResetPasswordViewModel()
        {
            Email = default!;
            Password = default!;
            PasswordConfirm = default!;
            Code = default!;
        }
    }
}
