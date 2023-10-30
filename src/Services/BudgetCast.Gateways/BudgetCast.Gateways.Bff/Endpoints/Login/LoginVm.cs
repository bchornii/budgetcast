namespace BudgetCast.Gateways.Bff.Endpoints.Login;

public class LoginVm
{
    public string AccessToken { get; set; }

    public LoginVm()
    {
        AccessToken = default!;
    }
}