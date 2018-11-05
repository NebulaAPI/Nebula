using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Nebula.Core.Services.Client;
using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Interfaces;
using Nebula.SDK.Objects;
using Nebula.SDK.Plugin;
using Nebula.SDK.Renderers;

namespace Nebula.Core.Factories
{
    public interface IRendererFactory
    {
        AbstractRenderer Get(LanguagePlugin languagePlugin, AbstractCompiler compiler, IRendererExtension rendererExtension);
    }
    
    public class RendererFactory : IRendererFactory
    {
        private Dictionary<Assembly, List<ILanguagePlugin>> _languagePlugins;
        
        public RendererFactory(IRegistryService registryService)
        {
            _languagePlugins = registryService.SearchForType<ILanguagePlugin>(registryService.LoadAllPlugins());
        }
        
        public AbstractRenderer Get(LanguagePlugin languagePlugin, AbstractCompiler compiler, IRendererExtension rendererExtension)
        {
            var pluginDll = _languagePlugins.Keys.FirstOrDefault(a => a.GetName().Name == languagePlugin.Name);
            if (pluginDll == null)
            {
                throw new Exception($"No plugins found that match {languagePlugin}");
            }
            var plugin = _languagePlugins[pluginDll].FirstOrDefault();
            if (plugin == null)
            {
                throw new Exception($"No plugin found that provides a compiler for {languagePlugin.Name}");
            }

            return plugin.GetRenderer(compiler, rendererExtension);
        }
    }
}