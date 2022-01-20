namespace BudgetCast.Identity.Api.ViewModels.Account
{
    public class LoginViewModel
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public LoginViewModel()
        {
            Email = default!;
            Password = default!;
        }
    }
}
