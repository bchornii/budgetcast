namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class LoginVm
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public LoginVm()
        {
            Email = default!;
            Password = default!;
        }
    }
}
