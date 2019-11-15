using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BudgetCast.Dashboard.Api.ViewModels
{
    public class RegisterViewModel
    {
        public string GivenName { get; set; }
        public string SurName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }

        public IdentityUser GetUser()
        {
            return new IdentityUser
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
