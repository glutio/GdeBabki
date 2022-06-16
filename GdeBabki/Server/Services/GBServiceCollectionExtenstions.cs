﻿using GdeBabki.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;

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

                var baseConnectionString = $"Data Source={Path.Combine(path, dbName)}.sqlite";
                var connectionString = new SqliteConnectionStringBuilder(baseConnectionString)
                {
                    Mode = SqliteOpenMode.ReadWriteCreate,
                    //Password = "hello"
                }.ToString();

                options
                    .UseSqlite(connectionString)
                    .LogTo(Console.WriteLine, LogLevel.Information);
            });

            return services;
        }

        public static IServiceCollection AddBabkiServices(this IServiceCollection services)
        {
            services.AddScoped<ImportService>();
            services.AddScoped<AccountsService>();
            services.AddScoped<TagsService>();
            return services;
        }
    }
}
