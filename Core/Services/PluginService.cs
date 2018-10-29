using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Nebula.SDK.Plugin;

namespace Nebula.Core.Services
{
    /// <summary>
    /// This service is responsible for installing, compiling and loading plugins
    /// </summary>
    public class PluginService
    {
        private string PluginFolder { get; set; }
        
        public PluginService(string pluginFolder)
        {
            PluginFolder = pluginFolder;
        }

        public T GetPlugin<T>()
        {
            // if (CompiledAssembly == null)
            // {
            //     GenerateInMemoryAssembly();
            // }
            // var type = CompiledAssembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Any(i => i.Name.Contains(typeof(T).Name)));
            // if (type == null)
            // {
            //     throw new Exception($"Could not find class that implements interface {typeof(T).Name}");
            // }
            // return (T)Activator.CreateInstance(type);
            return default(T);
        }
    }
}