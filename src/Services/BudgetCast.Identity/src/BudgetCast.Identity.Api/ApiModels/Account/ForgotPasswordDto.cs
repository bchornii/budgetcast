namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class ForgotPasswordDto
    {
        public string Email { get; set; }

        public ForgotPasswordDto()
        {
            Email = default!;
        }
    }
}
