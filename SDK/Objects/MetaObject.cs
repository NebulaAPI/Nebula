using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nebula.SDK.Objects
{
    public class MetaObject
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string TempFolder { get; set; }
        public Dictionary<string, string> Dependencies { get; set; }
    }
}