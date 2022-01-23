using Microsoft.AspNetCore.Identity;

namespace BudgetCast.Identity.Api.Database.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
