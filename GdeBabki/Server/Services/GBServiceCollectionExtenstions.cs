using GdeBabki.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Data.Sqlite;
using System.Security.Claims;
using GdeBabki.Server.Auth;

namespace GdeBabki.Server.Services
{
    public static class GBServiceCollectionExtensions
    {
        public static IServiceCollection AddUserDbFactory(this IServiceCollection services)
        {
            services.AddDbContextFactory<BabkiDbContext>((provider, options) =>
            {
                var userProvider = provider.GetService<UserService>();

                if (userProvider.DBName.Equals("GdeBabki", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException($"Reserved DB Name - {userProvider.DBName}");
                }

                var baseConnectionString = $"Data Source={userProvider.DBFilePath}.sqlite";
                var connectionString = new SqliteConnectionStringBuilder(baseConnectionString)
                {
                    Mode = SqliteOpenMode.ReadWriteCreate,
                    Password = userProvider.DBPassword
                }.ToString();

                options
                    .UseSqlite(connectionString)
                    .LogTo(Console.WriteLine, LogLevel.Information);
            }, ServiceLifetime.Scoped);

            return services;
        }


        public static IServiceCollection AddBabkiServices(this IServiceCollection services)
        {
            services.AddSingleton<UserService>();
            services.AddScoped<ImportService>();
            services.AddScoped<AccountsService>();
            services.AddScoped<TagsService>();
            services.AddScoped<DatabaseService>();

            return services;
        }
    }
}
