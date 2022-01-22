using System.Security.Claims;
using BudgetCast.Identity.Api.Database.Models;

namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class RegisterDto
    {
        public string GivenName { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public RegisterDto()
        {
            GivenName = default!;
            SurName = default!;
            Email = default!;
            Password = default!;
            PasswordConfirm = default!;
        }

        public ApplicationUser GetUser()
        {
            return new ApplicationUser
            {
                UserName = Email,
                Email = Email,
                GivenName = GivenName,
                LastName = SurName,
                IsActive = false,
            };
        }
    }
}
