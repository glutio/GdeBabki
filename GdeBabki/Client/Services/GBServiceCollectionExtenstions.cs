﻿using GdeBabki.Client.Pages;
using GdeBabki.Client.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Radzen;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GdeBabki.Client.Services
{
    public static class GBServiceCollectionExtensions
    {
        public static IServiceCollection AddBabkiServices(this IServiceCollection services)
        {
            services.AddScoped<ImportApi>();
            services.AddScoped<AccountsApi>();
            services.AddTransient<AccountsViewModel>();
            services.AddTransient<BanksViewModel>();
            services.AddTransient<NewAccountViewModel>();

            return services;
        }

        public async static Task<string> OpenInput(this DialogService dialogService, string title, string value)
        {
            var result = await dialogService.OpenAsync<InputDialog>(title, new Dictionary<string, object>() { { "Value", value } });
            return result;
        }
    }

}