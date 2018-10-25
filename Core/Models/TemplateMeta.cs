using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nebula.Models
{
    public class TemplateFile
    {
        public string OriginalName { get; set; }
        public string NewName { get; set; }
    }
    
    public class TemplateMeta
    {
        public string EntityLocation { get; set; }
        public string ClientLocation { get; set; }
        public string SourceFolder { get; set; }
        public string TemplateLocation { get; set; }
        public string PluginLocation { get; set; }
        public List<TemplateFile> FilesToRename { get; set; }
        [JsonIgnore] public string TemplatePath { get; set; }
        [JsonIgnore] public LibraryTemplate TemplateData { get; set; }

        public TemplateMeta()
        {
            SourceFolder = "";
        }
    }
}