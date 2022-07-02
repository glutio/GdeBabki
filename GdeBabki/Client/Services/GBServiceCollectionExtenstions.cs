using GdeBabki.Client.Pages;
using GdeBabki.Client.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeBabki.Client.Services
{
    public static class GBServiceCollectionExtensions
    {
        public static IServiceCollection AddBabkiServices(this IServiceCollection services)
        {
            services.AddSingleton<UserService>();
            services.AddSingleton<ErrorService>();

            services.AddSingleton<ImportApi>();
            services.AddSingleton<AccountsApi>();
            services.AddSingleton<TagsApi>();
            services.AddSingleton<UserApi>();


            services.AddTransient<AccountsViewModel>();
            services.AddTransient<BanksViewModel>();
            services.AddTransient<EditAccountViewModel>();
            services.AddTransient<ImportViewModel>();
            services.AddTransient<ErrorViewModel>();
            services.AddTransient<EditTagsViewModel>();
            services.AddTransient<PopupViewModel>();
            services.AddTransient<LoginViewModel>();

            services.AddSingleton<ReviewViewModel>();
            services.AddSingleton<AnalysisViewModel>();

            return services;
        }

        public async static Task<string> OpenInput(this DialogService dialogService, string title, string caption, string value)
        {
            var result = await dialogService.OpenAsync<InputDialog>(title, new Dictionary<string, object>() { { "Value", value }, { "Caption", caption } });
            return result;
        }
    }
}
