﻿using Microsoft.AspNetCore.Identity;

namespace BudgetCast.Identity.Api.Database.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsActive { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }

        public long? Tenant { get; set; }
    }
}
