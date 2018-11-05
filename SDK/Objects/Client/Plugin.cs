using System;
using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Objects.Client
{
    public class Plugin : BasePlugin
    {
        public Plugin() {}
        
        public List<PluginVersion> Versions { get; set; }
        
    }
}