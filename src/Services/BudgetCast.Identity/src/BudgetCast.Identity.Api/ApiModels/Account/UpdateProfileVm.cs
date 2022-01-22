namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class UpdateProfileVm
    {
        public string GivenName { get; set; }

        public string SurName { get; set; }

        public UpdateProfileVm()
        {
            GivenName = default!;
            SurName = default!;
        }
    }
}
