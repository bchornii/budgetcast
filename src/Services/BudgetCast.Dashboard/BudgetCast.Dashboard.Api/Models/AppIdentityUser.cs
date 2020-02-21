using Microsoft.AspNetCore.Identity;

namespace BudgetCast.Dashboard.Api.Models
{
    public class AppIdentityUser : IdentityUser
    {
        public string ProfileImageLink { get; set; }
        public string ProfileImageThumbnailLink { get; set; }
    }
}
