using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Newtonsoft.Json;
using Nebula.SDK.Objects;
using Nebula.SDK.Util;
using Nebula.SDK.Objects.Client;

namespace Nebula.Core.Services.Client
{
    public class TemplateService
    {
        private string ManifestRepo { get; set; }

        private Project CurrentProject { get; set; }

        private string ManifestFile { get; set; }

        private RegistryService RegistryService { get; set; }
        
        public TemplateService(Project project, RegistryService registryService = null)
        {
            ManifestRepo = NebulaConfig.TemplateManifestRepo;
            CurrentProject = project;
            ManifestFile = Path.Combine(NebulaConfig.TemplateDirectory, "template-manifest.json");
            RegistryService = registryService ?? new RegistryService();
        }

        public Template InstallTemplate(string name)
        {
            var manifestData = LoadLocalManifest();
            var template = RegistryService.InstallTemplate(name);
            manifestData.Templates.Add(template);
            SaveLocalManifest(manifestData);

            return template;
        }

        public TemplateManifest LoadLocalManifest()
        {
            if (!File.Exists(ManifestFile))
            {
                CreateEmptyManifest();
            }
            return JsonConvert.DeserializeObject<TemplateManifest>(File.ReadAllText(ManifestFile));
        }

        public void SaveLocalManifest(TemplateManifest manifest)
        {
            File.WriteAllText(ManifestFile, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        public void CreateEmptyManifest()
        {
            var manifest = new TemplateManifest();
            File.WriteAllText(ManifestFile, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        public List<Template> GetLocalTemplates()
        {
            if (!File.Exists(ManifestFile))
            {
                CreateEmptyManifest();
            }

            var manifestData = JsonConvert.DeserializeObject<TemplateManifest>(File.ReadAllText(ManifestFile));
            return manifestData.Templates;
        }

        public void RenderTemplateList()
        {
            var templates = GetLocalTemplates();
            var table = new ConsoleTable("Name", "Description", "Added");
            
            foreach (var t in templates)
            {
                var includedInProject = CurrentProject.Templates.Keys.Contains(t.Name);
                table.AddRow(t.Name, t.Description, includedInProject ? '\u2714': ' ');
            }
            table.Write(Format.Minimal);
        }

        public Template GetTemplate(string templateName)
        {
            return GetLocalTemplates().FirstOrDefault(t => t.Name == templateName);
        }

        public TemplateMeta GetTemplateMeta(string templateName)
        {
            var templates = GetLocalTemplates();
            var template = templates.FirstOrDefault(t => t.Name == templateName);
            if (template != null && CurrentProject.Templates.Keys.Contains(templateName))
            {
                var templatePath = Path.Combine(CurrentProject.TemplateDirectory, templateName);
                if (!Directory.Exists(templatePath))
                {
                    throw new System.Exception("Could not find template directory: " + templatePath);
                }
                var templateConfigFile = Path.Combine(templatePath, "nebula-meta.json");
                if (!File.Exists(templateConfigFile))
                {
                    throw new System.Exception("Could not find template meta file: " + templateConfigFile);
                }
                var templateMeta = JsonConvert.DeserializeObject<TemplateMeta>(File.ReadAllText(templateConfigFile));
                if (templateMeta == null)
                {
                    throw new System.Exception("Error reading template meta file: " + templateConfigFile);
                }
                templateMeta.TemplatePath = templatePath;
                templateMeta.TemplateData = template;
                return templateMeta;
            }

            return null;
        }

        public void CustomizeTemplate(string templatePath, string templateName)
        {
            var templateMeta = GetTemplateMeta(templateName);
            foreach (var file in templateMeta.Configuration.FilesToRename.Keys)
            {
                var fileToUpdate = Path.Combine(templatePath, file);
                if (File.Exists(fileToUpdate))
                {
                    var newFileName = templateMeta.Configuration.FilesToRename[file].Replace("%%NAME%%", CurrentProject.Name);
                    File.Move(fileToUpdate, Path.Combine(templatePath, newFileName));
                }
            }

            foreach (var f in Directory.GetFiles(templatePath))
            {
                var fileContent = File.ReadAllText(f);
                File.WriteAllText(f, 
                    fileContent
                        .Replace("%%NAME%%", CurrentProject.Name)
                        .Replace("%%VERSION%%", CurrentProject.Version)
                        .Replace("%%PROPERNAME%%", CurrentProject.GetProperName())
                );
            }
        }
    }
}