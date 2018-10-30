using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Plugin;
using Nebula.SDK.Objects;
using Nebula.Core.Services.Client;
using System.Collections.Generic;
using Nebula.SDK.Interfaces;
using System.Linq;
using System.Reflection;
using System;

namespace Nebula.Core.Factories
{
    /// <summary>
    /// 
    /// </summary>
    public class CompilerFactory
    {
        private RegistryService _registryService;
        private Dictionary<Assembly, List<ILanguagePlugin>> _languagePlugins;
        
        public CompilerFactory(RegistryService registryService, Dictionary<Assembly, List<ILanguagePlugin>> languagePlugins)
        {
            _registryService = registryService;
            _languagePlugins = languagePlugins;
        }
        
        /// <summary>
        /// For the specified language and language plugin, find the appropriate object and return it
        /// </summary>
        /// <param name="language">The language to use</param>
        /// <param name="languagePlugin">The specific language plugin to use</param>
        /// <returns></returns>
        public AbstractCompiler Get(string language, string languagePlugin)
        {
            var pluginDll = _languagePlugins.Keys.FirstOrDefault(a => a.GetName().Name == languagePlugin);
            if (pluginDll == null)
            {
                throw new Exception($"No plugins found that match {languagePlugin}");
            }
            var compiler = _languagePlugins[pluginDll].FirstOrDefault(p => p.GetLanguageName() == language);
            if (compiler == null)
            {
                throw new Exception($"No plugin found that provides a compiler for {language}");
            }

            return compiler.GetCompiler();
        }
    }
}