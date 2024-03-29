﻿namespace BudgetCast.Identity.Api.ApiModels.Account
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string PasswordConfirm { get; set; }

        public string Code { get; set; }

        public ResetPasswordDto()
        {
            Email = default!;
            Password = default!;
            PasswordConfirm = default!;
            Code = default!;
        }
    }
}
