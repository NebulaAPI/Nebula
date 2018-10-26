using System.Collections.Generic;
using Nebula.Core.Util;
using Newtonsoft.Json;

namespace Nebula.Core.Models
{
    public class Project
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Company { get; set; }
        public string Version { get; set; }
        public string Manifest { get; set; }
        public List<string> Templates { get; set; }
        [JsonIgnore]
        public string SourceDirectory { get; set; }
        [JsonIgnore]
        public string TemplateDirectory { get; set; }
        [JsonIgnore]
        public string ProjectDirectory { get; set; }
        [JsonIgnore]
        public string OutputDirectory { get; set; }
        [JsonIgnore]
        public string ManifestDirectory { get; set; }

        public Project()
        {
            Templates = new List<string>();
        }

        public string GetProperName()
        {
            return Name.Replace("-", " ").Replace("_", " ").ToProperCase().ToPascalCase();
        }

    }
}