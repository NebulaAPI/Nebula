using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Plugin;
using Nebula.SDK.Objects;
using Nebula.Core.Services.Client;
using System.Collections.Generic;
using Nebula.SDK.Interfaces;
using System.Linq;
using System.Reflection;
using System;
using Nebula.SDK.Objects.Shared;

namespace Nebula.Core.Factories
{
    public interface ICompilerFactory
    {
        AbstractCompiler Get(LanguagePlugin languagePlugin);
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class CompilerFactory : ICompilerFactory
    {
        private Dictionary<Assembly, List<ILanguagePlugin>> _languagePlugins;
        
        public CompilerFactory(IRegistryService registryService)
        {
            _languagePlugins = registryService.SearchForType<ILanguagePlugin>(registryService.LoadAllPlugins());
        }
        
        /// <summary>
        /// For the specified language and language plugin, find the appropriate object and return it
        /// </summary>
        /// <param name="language">The language to use</param>
        /// <param name="languagePlugin">The specific language plugin to use</param>
        /// <returns></returns>
        public AbstractCompiler Get(LanguagePlugin languagePlugin)
        {
            var pluginDll = _languagePlugins.Keys.FirstOrDefault(a => a.GetName().Name == languagePlugin.Name);
            if (pluginDll == null)
            {
                throw new Exception($"No plugins found that match {languagePlugin}");
            }
            var compiler = _languagePlugins[pluginDll].FirstOrDefault();
            if (compiler == null)
            {
                throw new Exception($"No plugin found that provides a compiler for {languagePlugin.Name}");
            }

            return compiler.GetCompiler();
        }
    }
}