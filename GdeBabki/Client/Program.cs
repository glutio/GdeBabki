using GdeBabki.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Radzen;
using System;
using System.Threading.Tasks;

namespace GdeBabki.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddHttpClient(Options.DefaultName, httpClient => { Console.WriteLine("BaseAddress" + builder.HostEnvironment.BaseAddress);
                httpClient.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress); });
            builder.Services.AddScoped<DialogService>();
            builder.Services.AddBabkiServices();

            await builder.Build().RunAsync();
        }
    }
}
