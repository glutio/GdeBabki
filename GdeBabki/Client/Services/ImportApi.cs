using GdeBabki.Shared.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GdeBabki.Client.Services
{
    public class ImportApi: ApiBase
    {
        public ImportApi(GBHttpClient httpClient) : base(httpClient)
        {
        }

        public async void ImportAsync(IBrowserFile file, string[] filter, Guid accountId)
        {
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            using var stream = file.OpenReadStream();
            content.Add(new StreamContent(stream), "file");
            content.Add(new StringContent(accountId.ToString()), "accountId");
            content.Add(new StringContent(String.Join(',', filter)), "filter");

            await httpClient.PostAsync("/api/Import", content);
        }

        public async Task<ImportViewModel> GetViewModelAsync()
        {            
            var model = await httpClient.GetFromJsonAsync<ImportViewModel>("/api/Import");
            return model;
        }
    }
}
