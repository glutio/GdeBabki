namespace GdeBabki.Client.Services
{
    public class ApiBase
    {
        protected readonly GBHttpClient httpClient;

        public ApiBase(GBHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
    }
}
