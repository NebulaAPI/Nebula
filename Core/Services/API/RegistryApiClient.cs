using System;
using System.Collections.Generic;
using Core.Services.API;
using Nebula.SDK.Objects;
using Nebula.SDK.Objects.Client;
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

        public List<Plugin> SearchPlugins(string query)
        {
            var req = new RestRequest("/plugin/search/{query}", Method.GET, DataFormat.Json);
            req.AddUrlSegment("query", query);
            var response = Client.Execute<List<Plugin>>(req);
            if (response.ErrorException != null)
            {
                throw new Exception(response.ErrorException.Message);
            }

            return response.Data;
        }

        public Plugin GetPlugin(string name)
        {
            var req = new RestRequest("/plugin/{name}", Method.GET, DataFormat.Json);
            req.AddUrlSegment("name", name);
            var response = Client.Execute<Plugin>(req);
            if (response.ErrorException != null)
            {
                throw new Exception(response.ErrorException.Message);
            }

            return response.Data;
        }
    }
}