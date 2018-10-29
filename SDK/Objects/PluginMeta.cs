using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nebula.SDK.Objects
{
    public class PluginMeta
    {
        public string Name { get; set; }
        public string Vendor { get; set; }
        public string Author { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
        [JsonIgnore]
        public string TempFolder { get; set; }
    }
}