namespace BudgetCast.Gateways.Bff.Models;

public class LoginVm
{
    public string AccessToken { get; set; }

    public LoginVm()
    {
        AccessToken = default!;
    }
}