using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PMIWeb.Areas.Identity.Data;
using PMIWeb.Models;

[assembly: HostingStartup(typeof(PMIWeb.Areas.Identity.IdentityHostingStartup))]
namespace PMIWeb.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddDbContext<PMIWebContext>(options =>
                    options.UseSqlite(
                        context.Configuration.GetConnectionString("PMIWebContextConnection")));

                services.AddDefaultIdentity<PMIWebUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<PMIWebContext>();

                services.ConfigureApplicationCookie(options =>
                {
                    options.LoginPath = $"/identity/account/login";
                    options.LogoutPath = $"/identity/account/logout";
                });
            });
        }
    }
}