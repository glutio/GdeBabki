using GdeBabki.Server.Data;
using Microsoft.Extensions.DependencyInjection;

namespace GdeBabki.Server.Services
{
    public static class GBServiceCollectionExtensions
    {
        public static IServiceCollection AddUserDbFactory(this IServiceCollection services)
        {
            services.AddDbContextFactory<BabkiDbContext>((provider, options) =>
            {
                var userService = provider.GetService<UserService>();
                userService.SetubDbContextOptions(options, false);
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
