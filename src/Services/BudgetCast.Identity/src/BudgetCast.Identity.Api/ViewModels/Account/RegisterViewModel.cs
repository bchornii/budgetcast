using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace BudgetCast.Identity.Api.ViewModels.Account
{
    public class RegisterViewModel
    {
        public string GivenName { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public RegisterViewModel()
        {
            GivenName = default!;
            SurName = default!;
            Email = default!;
            Password = default!;
            PasswordConfirm = default!;
        }

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
