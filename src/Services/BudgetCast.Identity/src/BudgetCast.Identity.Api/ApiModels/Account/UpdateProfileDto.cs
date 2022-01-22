namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class UpdateProfileDto
    {
        public string GivenName { get; set; }

        public string SurName { get; set; }

        public UpdateProfileDto()
        {
            GivenName = default!;
            SurName = default!;
        }
    }
}
