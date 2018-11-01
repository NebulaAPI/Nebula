using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LibGit2Sharp;
using Nebula.Core.Services.API;
using Nebula.SDK.Exceptions;
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
        public List<Plugin> SearchPlugins(string query)
        {
            return Client.SearchPlugins(query);
        }

        /// <summary>
        /// Search for templates in official registry
        /// </summary>
        /// <param name="query">terms to search for</param>
        /// <returns>Template object</returns>
        public List<Template> SearchTemplates(string query)
        {
            return Client.SearchTemplates(query);
        }

        /// <summary>
        /// Get the details for a specific plugin from the official registry
        /// </summary>
        /// <param name="name">The plugin to retrieve</param>
        /// <returns>Plugin object</returns>
        public Plugin GetPlugin(string name)
        {
            return Client.GetPlugin(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Template GetTemplate(string name)
        {
            return Client.GetTemplate(name);
        }

        /// <summary>
        /// Locally install the specified plugin from the official registry
        /// </summary>
        /// <param name="name">The plugin to install</param>
        public void InstallPlugin(string name)
        {
            var plugin = GetPlugin(name);
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
        /// Locally install the specified plugin from the official registry
        /// </summary>
        /// <param name="name">The plugin to install</param>
        public Template InstallTemplate(string name)
        {
            var template = GetTemplate(name);
            if (template == null)
            {
                throw new Exception("Could not find tenmplate: " + name);
            }

            var installedTemplates = GetInstalledTemplates();
            if (installedTemplates.Any(p => p.Name == name))
            {
                return template;
            }

            var templateDir = Path.Combine(NebulaConfig.TemplateDirectory, template.Name);
            Repository.Clone(template.RepositoryUrl, templateDir);
            return template;
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
            return CompilationService.CompileLocal(meta.Name, assemblyFile, pluginFiles.ToArray());
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
        public Dictionary<Assembly, List<T>> SearchForType<T>(List<Assembly> assemblies)
        {
            var result = new Dictionary<Assembly, List<T>>();
            var searchActions = new List<Func<Assembly, IEnumerable<T>>> {
                (a) => a.GetTypes().Where(t => t.GetInterfaces().Any(i => i.Name.Contains(typeof(T).Name))).Select(t => (T)Activator.CreateInstance(t)),
                (a) => a.GetTypes().Where(t => t.BaseType == typeof(T)).Select(t => (T)Activator.CreateInstance(t)),
                (a) => a.GetTypes().Where(t => t == typeof(T)).Select(t => (T)Activator.CreateInstance(t))
            };

            foreach (var assembly in assemblies)
            {
                var types = searchActions.Select(a => a(assembly));
                foreach (var t in types)
                {
                    if (result.ContainsKey(assembly))
                    {
                        result[assembly].AddRange(t.ToList());
                    }
                    else
                    {
                        result.Add(assembly, new List<T>(t));
                    }
                }
            }
                
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

        /// <summary>
        /// Gets meta data for currently installed plugins
        /// </summary>
        /// <returns>List of PluginMeta objects</returns>
        public List<TemplateMeta> GetInstalledTemplates()
        {
            var result = new List<TemplateMeta>();
            foreach (var dir in Directory.GetDirectories(NebulaConfig.TemplateDirectory))
            {
                var metaFile = Path.Combine(dir, "nebula-meta.json");
                result.Add(JsonConvert.DeserializeObject<TemplateMeta>(File.ReadAllText(metaFile)));
            }
            return result;
        }
    }
}