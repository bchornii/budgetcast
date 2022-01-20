namespace BudgetCast.Identity.Api.ViewModels.Account
{
    public class UpdateProfileViewModel
    {
        public string GivenName { get; set; }

        public string SurName { get; set; }

        public UpdateProfileViewModel()
        {
            GivenName = default!;
            SurName = default!;
        }
    }
}
