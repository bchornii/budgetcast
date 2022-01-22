﻿using System.Security.Claims;
using BudgetCast.Identity.Api.Database.Models;

namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class RegisterVm
    {
        public string GivenName { get; set; }

        public string SurName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public RegisterVm()
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
                FirstName = GivenName,
                LastName = SurName,
                IsActive = false,
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
