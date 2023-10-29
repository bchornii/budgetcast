namespace BudgetCast.Gateways.Bff.Models;

public class RefreshAccessTokenVm
{
    public string AccessToken { get; set; }

    public RefreshAccessTokenVm()
    {
        AccessToken = default!;
    }
}