using System;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using InvoiceManager.Data;

namespace InvoiceManager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = BuildWebHost(args);

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                context.Database.Migrate();

                var config = host.Services.GetRequiredService<IConfiguration>();
                var testUserPw = config[Resources.ApplicationTexts.SeedUserPW];

                try
                {
                    SeedData.Initialize(services, testUserPw).Wait();
                }
                catch (Exception exception)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception, Resources.ApplicationTexts.ErrorWhileSeedingTheDatabase);
                    throw;
                }
            }

            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) => WebHost.CreateDefaultBuilder(args)
                                                                     .UseStartup<Startup>()
                                                                     .Build();
    }
}