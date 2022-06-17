using GdeBabki.Shared;
using GdeBabki.Shared.DTO;
using System.Net.Http;
using System.Net.Http.Headers;

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
