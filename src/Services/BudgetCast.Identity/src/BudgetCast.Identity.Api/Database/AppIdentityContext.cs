using BudgetCast.Identity.Api.Database.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BudgetCast.Identity.Api.Database
{
    public class AppIdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private const string DefaultSchema = "dbo";

        public AppIdentityContext(DbContextOptions<AppIdentityContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users", DefaultSchema);
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("Roles", DefaultSchema);
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.ToTable("RoleClaims", DefaultSchema);
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.ToTable("UserRoles", DefaultSchema);
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.ToTable("UserClaims", DefaultSchema);
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.ToTable("UserLogins", DefaultSchema);
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.ToTable("UserTokens", DefaultSchema);
            });
        }
    }
}
