using GdeBabki.Client.Pages;
using GdeBabki.Shared.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GdeBabki.Client.Services
{
    public class UserApi : ApiBase
    {
        public UserApi(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

        public static LoginInfo LoginInfo { get; set; }

        public async Task Login()
        {
            using var httpClient = httpFactory.CreateClient();

            var response = await httpClient.GetAsync("api/User");            
            response.EnsureSuccessStatusCode();
        }

        public async Task Create()
        {
            using var httpClient = httpFactory.CreateClient();

            var response = await httpClient.PostAsync("api/User", null);
            response.EnsureSuccessStatusCode();
        }
    }
}
