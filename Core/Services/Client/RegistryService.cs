using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LibGit2Sharp;
using Nebula.Core.Services.API;
using Nebula.SDK.Objects;
using Nebula.SDK.Objects.Client;
using Nebula.SDK.Util;
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

        public Assembly Compile(string name)
        {
            var meta = GetInstalledPlugins().FirstOrDefault(p => p.Name == name);
            if (meta == null)
            {
                throw new Exception($"Plugin {name} not installed.");
            }

            var pluginDir = Path.Combine(NebulaConfig.PluginDirectory, meta.Name);
            var pluginFiles = new List<string>();
            var assemblyFile = Path.Combine(pluginDir, $"{meta.Name}.dll");
            FileUtil.GenerateFileList(pluginDir, pluginFiles, ".cs", (f) => f);
            return CompilationService.CompileLocal(assemblyFile, pluginFiles.ToArray());
        }

        public List<Assembly> LoadAllPlugins()
        {
            var output = new List<Assembly>();
            var plugins = GetInstalledPlugins();
            foreach (var plugin in plugins)
            {   
                var dllFile = Path.Combine(NebulaConfig.PluginDirectory, plugin.Name, $"{plugin.Name}.dll");
                if (!File.Exists(dllFile))
                {
                    output.Add(Compile(plugin.Name));
                    continue;
                }
                output.Add(Assembly.LoadFile(dllFile));
            }

            return output;
        }

        public List<T> SearchForType<T>(List<Assembly> assemblies) where T : new()
        {
            var result = new List<T>();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Any(i => i.Name.Contains(typeof(T).Name)));
                if (type == null)
                {
                    type = assembly.GetTypes();
                }
            }
            
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