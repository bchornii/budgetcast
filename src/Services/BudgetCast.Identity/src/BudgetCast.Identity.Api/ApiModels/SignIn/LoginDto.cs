namespace BudgetCast.Identity.Api.ApiModels.SignIn
{
    public class LoginDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public LoginDto()
        {
            Email = default!;
            Password = default!;
        }
    }
}
