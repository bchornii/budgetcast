namespace BudgetCast.Identity.Api.ApiModels.Token
{
    public record TokenResponseVm(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime);
}
