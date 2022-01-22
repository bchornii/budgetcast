namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class ForgotPasswordVm
    {
        public string Email { get; set; }

        public ForgotPasswordVm()
        {
            Email = default!;
        }
    }
}
