using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GdeBabki.Client.Services
{
    public class UserApi : ApiBase
    {
        public UserApi(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

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
            if (response.StatusCode == HttpStatusCode.Conflict)
            {
                throw new DuplicateNameException("A user with this name already exists");
            }
            response.EnsureSuccessStatusCode();
        }
    }
}
