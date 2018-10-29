using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Nebula.Core.Services.API;
using Nebula.SDK.Objects;
using Nebula.SDK.Objects.Client;
using Newtonsoft.Json;

namespace Nebula.Core.Services.Client
{
    public class RegistryService
    {
        private RegistryApiClient Client { get; set; }
        private CompilationService CompilationService { get; set; }

        public RegistryService(RegistryApiClient client = null, CompilationService compilationService = null)
        {
            Client = client ?? new RegistryApiClient();
            CompilationService = compilationService ?? new CompilationService();
        }

        public List<Plugin> Search(string query)
        {
            return Client.SearchPlugins(query);
        }

        public Plugin Get(string name)
        {
            return Client.GetPlugin(name);
        }

        public void Install(string name)
        {
            var plugin = Get(name);
            if (plugin == null)
            {
                throw new Exception("Could not find plugin: " + name);
            }

            var installedPlugins = GetInstalledPlugins();
            if (installedPlugins.Any(p => p.Name == name))
            {
                throw new Exception($"{name} already installed");
            }

            var pluginDir = Path.Combine(NebulaConfig.PluginDirectory, plugin.Name);
            Repository.Clone(plugin.RepositoryUrl, pluginDir);
        }

        public List<PluginMeta> GetInstalledPlugins()
        {
            var result = new List<PluginMeta>();
            foreach (var dir in Directory.GetDirectories(NebulaConfig.PluginDirectory))
            {
                var metaFile = Path.Combine(dir, "nebula-meta.json");
                result.Add(JsonConvert.DeserializeObject<PluginMeta>(File.ReadAllText(metaFile)));
            }
            return result;
        }
    }
}