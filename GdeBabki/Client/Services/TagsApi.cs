using GdeBabki.Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GdeBabki.Client.Services
{
    public class TagsApi: ApiBase
    {
        public TagsApi(IHttpClientFactory httpFactory): base(httpFactory)
        {

        }

        public async Task AddTagAsync(TransactionTag transactionTag)
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync("api/Tags", transactionTag);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteTagAsync(string tagId, Guid transactionId)
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.DeleteAsync($"api/Tags?transactionId={transactionId}&tagId={tagId}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<List<string>> SuggestTagsAsync(Guid transactionId)
        {
            var httpClient = httpFactory.CreateClient();
            var response = await httpClient.GetAsync($"api/Tags/Suggested?transactionId={transactionId}");
            response.EnsureSuccessStatusCode();

            var model = await response.Content.ReadFromJsonAsync<string[]>();

            return model.ToList();
        }

        public async Task AddTagsAsync(IEnumerable<TransactionTag> transactionTags)
        {
            foreach (var transactionTag in transactionTags)
            {
                await AddTagAsync(transactionTag);
            }
        }

        public async Task DeleteTagsAsync(string tagId, IEnumerable<Guid> transactionIds)
        {
            foreach (var transactionId in transactionIds)
            {
                await DeleteTagAsync(tagId, transactionId);
            }
        }
    }
}
