using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nebula.Models
{
    public class Project
    {
        public string Name { get; set; }
        public List<Author> Authors { get; set; }
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

        public Project()
        {
            Authors = new List<Author>();
            Templates = new List<string>();
        }

    }
}