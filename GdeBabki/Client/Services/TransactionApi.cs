using System.Net.Http;

namespace GdeBabki.Client.Services
{
    public class TransactionApi : ApiBase
    {
        public TransactionApi(IHttpClientFactory httpFactory) : base(httpFactory)
        {
        }

    }
}
