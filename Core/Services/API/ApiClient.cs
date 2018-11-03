using System.Net;
using RestSharp;

namespace Core.Services.API
{
    public class ApiClient
    {
        protected RestClient _client;

        protected ApiClient(string baseUri)
        {
            _client = new RestClient(baseUri);
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
    }
}