namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class UpdateProfileVm
    {
        public string AccessToken { get; set; }

        public UpdateProfileVm()
        {
            AccessToken = default!;
        }
    }
}
