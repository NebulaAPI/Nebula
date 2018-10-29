using System.Net;
using RestSharp;

namespace Core.Services.API
{
    public class ApiClient
    {
        protected RestClient Client { get; set; }

        protected ApiClient(string baseUri)
        {
            Client = new RestClient(baseUri);
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;
        }
    }
}