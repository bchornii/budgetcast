namespace BudgetCast.Gateways.Bff.Endpoints.Login;

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