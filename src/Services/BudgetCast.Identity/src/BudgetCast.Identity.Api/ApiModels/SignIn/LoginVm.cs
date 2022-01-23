namespace BudgetCast.Identity.Api.ApiModels.SignIn
{
    public class LoginVm
    {
        public string AccessToken { get; set; }

        public LoginVm()
        {
            AccessToken = default!;
        }
    }
}
