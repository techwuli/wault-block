using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using WaultBlock.Data;
using WaultBlock.Identities;

namespace WaultBlock.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            var dbContext = host.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
            WaultIdentityService.RegisterWalletType(dbContext);
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
