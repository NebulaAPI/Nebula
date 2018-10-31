using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nebula.SDK.Objects
{
    public class PluginMeta : MetaObject
    {
        public string Vendor { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
}