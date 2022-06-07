using System.Net.Http;

namespace GdeBabki.Client.Services
{
    public class ApiBase
    {
        protected readonly IHttpClientFactory httpFactory;

        public ApiBase(IHttpClientFactory httpFactory)
        {
            this.httpFactory = httpFactory;
        }
    }
}
