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
    /// <summary>
    /// Methods for working with the official plugin registry from client applications
    /// </summary>
    public class RegistryService
    {
        private RegistryApiClient Client { get; set; }
        private CompilationService CompilationService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="compilationService"></param>
        public RegistryService(RegistryApiClient client = null, CompilationService compilationService = null)
        {
            Client = client ?? new RegistryApiClient();
            CompilationService = compilationService ?? new CompilationService();
        }

        /// <summary>
        /// Search for plugins in official registry
        /// </summary>
        /// <param name="query">terms to search for</param>
        /// <returns>Plugin object</returns>
        public List<Plugin> Search(string query)
        {
            return Client.SearchPlugins(query);
        }

        /// <summary>
        /// Get the details for a specific plugin from the official registry
        /// </summary>
        /// <param name="name">The plugin to retrieve</param>
        /// <returns>Plugin object</returns>
        public Plugin Get(string name)
        {
            return Client.GetPlugin(name);
        }

        /// <summary>
        /// Locally install the specified plugin from the official registry
        /// </summary>
        /// <param name="name">The plugin to install</param>
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

        /// <summary>
        /// Compile the specified plugin installed from the official registry.
        /// Performed automatically upon install.
        /// </summary>
        /// <param name="name">Plugin to compile</param>
        /// <returns>Fully loaded Assembly reference</returns>
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

        /// <summary>
        /// Load the compiled dll files for all currently installed plugins
        /// </summary>
        /// <returns>List of Assembly objects</returns>
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

        /// <summary>
        /// Search the provided list of assemblies for the specified type.
        /// List of assemblies can come from LoadAllPlugins()
        /// </summary>
        /// <param name="assemblies">Output from LoadAllPlugins()</param>
        /// <typeparam name="T">The type to search for</typeparam>
        /// <returns>List of instanced objects of the specified type</returns>
        public List<T> SearchForType<T>(List<Assembly> assemblies)
        {
            var result = new List<T>();
            var searchActions = new List<Func<Assembly, IEnumerable<T>>> {
                (a) => a.GetTypes().Where(t => t.GetInterfaces().Any(i => i.Name.Contains(typeof(T).Name))).Select(t => (T)Activator.CreateInstance(t)),
                (a) => a.GetTypes().Where(t => t.BaseType == typeof(T)).Select(t => (T)Activator.CreateInstance(t)),
                (a) => a.GetTypes().Where(t => t == typeof(T)).Select(t => (T)Activator.CreateInstance(t))
            };

            assemblies
                .ForEach(assembly => searchActions
                    .Select(a => a(assembly))
                    .ToList()
                    .ForEach(sa => result.AddRange(sa))
                );
                
            return result;
        }

        /// <summary>
        /// Gets meta data for currently installed plugins
        /// </summary>
        /// <returns>List of PluginMeta objects</returns>
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