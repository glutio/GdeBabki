using GdeBabki.Client.ViewModel;
using GdeBabki.Shared;
using GdeBabki.Shared.ViewModels;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.IO;
using System.Linq;
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

        public async Task ImportAsync(Guid accountId, Stream stream, GBColumnName?[] filter)
        {
            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");

            content.Add(new StreamContent(stream), "\"file\"", "filename.ext");
            content.Add(new StringContent(accountId.ToString()), "\"accountId\"");
            content.Add(new StringContent(String.Join(',', filter.Cast<int?>())), "\"filter\"");

            await httpClient.PostAsync("/api/Import", content);
        }
    }
}
