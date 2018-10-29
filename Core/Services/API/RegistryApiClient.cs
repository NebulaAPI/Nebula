using Core.Services.API;
using Nebula.SDK.Objects;
using RestSharp;

namespace Nebula.Core.Services.API
{
    public class RegistryApiClient : ApiClient
    {
        public RegistryApiClient() : base("https://localhost:5001/api")
        {

        }

        public void ImportPlugin(string repoUrl)
        {
            var req = new RestRequest("/plugin", Method.POST, DataFormat.Json);
            req.AddBody(repoUrl);
            Client.Execute(req);
        }

        public PluginMeta GetPluginMeta(string name)
        {
            var req = new RestRequest("/plugin/{name}", Method.GET, DataFormat.Json);
            req.AddUrlSegment("name", name);
            return Client.Execute<PluginMeta>(req).Data;
        }
    }
}