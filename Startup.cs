using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using InvoiceManager.Authorization;
using InvoiceManager.Data;
using InvoiceManager.Services;

namespace InvoiceManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
        }

        public IConfiguration Configuration { get; }
        private IHostingEnvironment Environment { get; }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            var skipHttps = Configuration.GetValue<bool>("LocalTest:skipHTTPS");

            services.Configure<MvcOptions>(options =>
            {
                if (Environment.IsDevelopment() && !skipHttps)
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                }
            });

            services.AddSingleton<IEmailSender, EmailSender>();
          
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.AddScoped<IAuthorizationHandler,
                                  InvoiceIsOwnerAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                                  InvoiceAdministratorsAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler,
                                  InvoiceManagerAuthorizationHandler>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvc();           
        }
    }
}
