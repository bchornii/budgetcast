namespace BudgetCast.Identity.Api.ApiModels.SignIn
{
    public class RefreshAccessTokenVm
    {
        public string AccessToken { get; set; }

        public RefreshAccessTokenVm()
        {
            AccessToken = default!;
        }
    }
}
