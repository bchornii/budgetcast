using System.Security.Claims;
using BudgetCast.Dashboard.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace BudgetCast.Dashboard.Api.ViewModels.Account
{
    public class RegisterViewModel
    {
        public string GivenName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public AppIdentityUser GetUser()
        {
            return new AppIdentityUser
            {
                UserName = Email,
                Email = Email
            };
        }

        public Claim[] GetClaims()
        {
            return new[]
            {
                new Claim(ClaimTypes.GivenName, GivenName),
                new Claim(ClaimTypes.Surname, SurName)
            };
        }
    }
}
