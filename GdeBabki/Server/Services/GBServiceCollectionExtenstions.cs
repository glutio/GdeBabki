using GdeBabki.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace GdeBabki.Server.Services
{
    public static class GBServiceCollectionExtensions
    {
        public static IServiceCollection AddTenantDbFactory(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddTransient(provider => provider.GetService<IHttpContextAccessor>().HttpContext?.User);
            services.AddSingleton<ITenantProvider, TenantProvider>();

            services.AddDbContextFactory<BabkiDbContext>((provider, options) =>
            {
                var dbName = provider.GetService<ITenantProvider>().DBName;

                if (dbName.Equals("GdeBabki", StringComparison.InvariantCultureIgnoreCase))
                {
                    throw new InvalidOperationException($"Unsupported Tenant DB Name - {dbName}");
                }

                var path = provider.GetService<IWebHostEnvironment>().IsDevelopment()
                    ? ""
                    : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                options.UseSqlite($"Data Source={Path.Combine(path, dbName)}.sqlite");
            });

            return services;
        }
    }
}
