using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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
    public interface IRegistryService
    {
        /// <summary>
        /// Search for plugins in official registry
        /// </summary>
        /// <param name="query">terms to search for</param>
        /// <returns>Plugin object</returns>
        List<Plugin> SearchPlugins(string query);

        /// <summary>
        /// Search for templates in official registry
        /// </summary>
        /// <param name="query">terms to search for</param>
        /// <returns>Template object</returns>
        List<Template> SearchTemplates(string query);

        /// <summary>
        /// Get the details for a specific plugin from the official registry
        /// </summary>
        /// <param name="name">The plugin to retrieve</param>
        /// <returns>Plugin object</returns>
        Plugin GetPlugin(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Template GetTemplate(string name);

        /// <summary>
        /// Locally install the specified plugin from the official registry
        /// </summary>
        /// <param name="name">The plugin to install</param>
        void InstallPlugin(string name);

        /// <summary>
        /// Locally install the specified plugin from the official registry
        /// </summary>
        /// <param name="name">The plugin to install</param>
        Template InstallTemplate(string name);

        /// <summary>
        /// Compile the specified plugin installed from the official registry.
        /// Performed automatically upon install.
        /// </summary>
        /// <param name="name">Plugin to compile</param>
        /// <returns>Fully loaded Assembly reference</returns>
        Assembly Compile(string name);

        /// <summary>
        /// Load the compiled dll files for all currently installed plugins
        /// </summary>
        /// <returns>List of Assembly objects</returns>
        List<Assembly> LoadAllPlugins(bool force = false);

        /// <summary>
        /// Search the provided list of assemblies for the specified type.
        /// List of assemblies can come from LoadAllPlugins()
        /// </summary>
        /// <param name="assemblies">Output from LoadAllPlugins()</param>
        /// <typeparam name="T">The type to search for</typeparam>
        /// <returns>List of instanced objects of the specified type</returns>
        Dictionary<Assembly, List<T>> SearchForType<T>(List<Assembly> assemblies);

        /// <summary>
        /// Gets meta data for currently installed plugins
        /// </summary>
        /// <returns>List of PluginMeta objects</returns>
        List<PluginMeta> GetInstalledPlugins(Func<PluginMeta, bool> predicate = null);

        /// <summary>
        /// Gets meta data for currently installed plugins
        /// </summary>
        /// <returns>List of PluginMeta objects</returns>
        List<TemplateMeta> GetInstalledTemplates(Func<TemplateMeta, bool> predicate = null);
    }
    
    public class RegistryService : IRegistryService
    {
        private RegistryApiClient _client;
        private ICompilationService _compilationService;
        private IFileUtil _fileUtil;
        private List<Assembly> _plugins;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="compilationService"></param>
        public RegistryService(
            RegistryApiClient client,
            ICompilationService compilationService,
            IFileUtil fileUtil
        ) {
            _client = client;
            _compilationService = compilationService;
            _fileUtil = fileUtil;
        }
        
        public List<Plugin> SearchPlugins(string query)
        {
            return _client.SearchPlugins(query);
        }
        
        public List<Template> SearchTemplates(string query)
        {
            return _client.SearchTemplates(query);
        }
        
        public Plugin GetPlugin(string name)
        {
            return _client.GetPlugin(name);
        }
        
        public Template GetTemplate(string name)
        {
            return _client.GetTemplate(name);
        }
        
        public void InstallPlugin(string name)
        {
            var plugin = GetPlugin(name);
            if (plugin == null)
            {
                throw new PluginNotFoundException(name);
            }

            var installedPlugins = GetInstalledPlugins();
            if (installedPlugins.Any(p => p.Name == name))
            {
                throw new AlreadyInstalledException(name);
            }

            var pluginDir = Path.Combine(NebulaConfig.PluginDirectory, plugin.Name);
            Repository.Clone(plugin.RepositoryUrl, pluginDir);
        }
        
        public Template InstallTemplate(string name)
        {
            var template = GetTemplate(name);
            if (template == null)
            {
                throw new TemplateNotFoundException(name);
            }

            var installedTemplates = GetInstalledTemplates();
            if (installedTemplates.Any(p => p.Name == name))
            {
                return template;
            }

            var templateDir = Path.Combine(NebulaConfig.TemplateDirectory, template.Name);
            Repository.Clone(template.RepositoryUrl, templateDir);

            try
            {
                InstallPlugin(template.LanguagePlugin.Name);
            }
            catch (AlreadyInstalledException)
            {

            }

            return template;
        }
        
        public Assembly Compile(string name)
        {
            var meta = GetInstalledPlugins().FirstOrDefault(p => p.Name == name);
            if (meta == null)
            {
                throw new PluginNotFoundException(name);
            }

            var pluginDir = Path.Combine(NebulaConfig.PluginDirectory, meta.Name);
            var pluginFiles = new List<string>();
            var assemblyFile = Path.Combine(pluginDir, $"{meta.Name}.dll");
            _fileUtil.GenerateFileList(pluginDir, pluginFiles, ".cs", (f) => f);
            return _compilationService.CompileLocal(meta.Name, assemblyFile, pluginFiles.ToArray());
        }
        
        public List<Assembly> LoadAllPlugins(bool force = false)
        {
            if (_plugins != null && _plugins.Count > 0 && !force)
            {
                return _plugins;
            }
            _plugins = new List<Assembly>();
            var plugins = GetInstalledPlugins();
            foreach (var plugin in plugins)
            {   
                var dllFile = Path.Combine(NebulaConfig.PluginDirectory, plugin.Name, $"{plugin.Name}.dll");
                if (!_fileUtil.FileExists(dllFile))
                {
                    _plugins.Add(Compile(plugin.Name));
                    continue;
                }

                try
                {
                    // Loading the assembly can fail for a variety of reasons. If it does, try to recompile it
                    _plugins.Add(Assembly.LoadFile(dllFile));
                }
                catch (Exception)
                {
                    _plugins.Add(Compile(plugin.Name));
                }
                
            }

            return _plugins;
        }
        
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
        
        public List<PluginMeta> GetInstalledPlugins(Func<PluginMeta, bool> predicate = null)
        {
            var result = new List<PluginMeta>();
            foreach (var dir in _fileUtil.GetDirectories(NebulaConfig.PluginDirectory))
            {
                var metaFile = Path.Combine(dir, "nebula-meta.json");
                result.Add(JsonConvert.DeserializeObject<PluginMeta>(_fileUtil.FileReadAllText(metaFile)));
            }

            if (predicate != null)
            {
                return result.Where(predicate).ToList();
            }

            return result;
        }
        
        public List<TemplateMeta> GetInstalledTemplates(Func<TemplateMeta, bool> predicate = null)
        {
            var result = new List<TemplateMeta>();
            foreach (var dir in _fileUtil.GetDirectories(NebulaConfig.TemplateDirectory))
            {
                var metaFile = Path.Combine(dir, "nebula-meta.json");
                result.Add(JsonConvert.DeserializeObject<TemplateMeta>(_fileUtil.FileReadAllText(metaFile)));
            }

            if (predicate != null)
            {
                return result.Where(predicate).ToList();
            }

            return result;
        }
    }
}