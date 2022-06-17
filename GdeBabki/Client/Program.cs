using GdeBabki.Client.Services;
using GdeBabki.Shared;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Radzen;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GdeBabki.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddHttpClient(Options.DefaultName, httpClient => 
            {
                httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
                httpClient.DefaultRequestHeaders.Authorization = GBAuthentication.GetAuthHeader(UserApi.LoginInfo);
            });

            builder.Services.AddBabkiServices();
            builder.Services.AddScoped<DialogService>();

            //AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            //{
            //    var errorService = builder.Services.BuildServiceProvider().GetService<ErrorService>();
            //    errorService.AddError(e.ToString());
            //};
            await builder.Build().RunAsync();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
