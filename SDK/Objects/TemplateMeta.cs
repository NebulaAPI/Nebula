using System.Collections.Generic;
using Newtonsoft.Json;

namespace Nebula.SDK.Objects
{
    public class TemplateConfiguration
    {
        public string EntityLocation { get; set; }
        public string ClientLocation { get; set; }
        public string SourceFolder { get; set; }
        public string ExtensionLocation { get; set; }
        public string PluginLocation { get; set; }
        public Dictionary<string, string> FilesToRename { get; set; }

        public TemplateConfiguration()
        {
            SourceFolder = "";
        }
    }

    public class LanguagePlugin
    {
        public string Name { get; set; }
        public string Version { get; set; }
    }
    
    public class TemplateMeta : MetaObject
    {
        public string Author { get; set; }
        public string Description { get; set; }
        public LanguagePlugin LanguagePlugin { get; set; }
        public TemplateConfiguration Configuration { get; set; }
        public string TemplateLocation { get; set; }
        [JsonIgnore] public string TemplatePath { get; set; }
        [JsonIgnore] public LibraryTemplate TemplateData { get; set; }
        
    }
}