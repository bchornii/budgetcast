using BudgetCast.Dashboard.Api.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BudgetCast.Dashboard.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                CreateWebHostBuilder(args).Build()
                    .MigrateDbContext<IdentityDbContext>((_, __) => { })
                    .Run();                
                return 0;
            }
            catch
            {
                return 1;                
            }            
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
