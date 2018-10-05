using System.Collections.Generic;
using System.IO;
using System.Linq;
using Nebula.Models;
using LibGit2Sharp;
using Newtonsoft.Json;

namespace Core.Services
{
    public class TemplateService
    {
        private string ManifestRepo { get; set; }

        private Project CurrentProject { get; set; }

        private string ManifestFile { get; set; }
        
        public TemplateService(Project project, string manifestRepo)
        {
            ManifestRepo = manifestRepo;
            CurrentProject = project;
            ManifestFile = Path.Combine(CurrentProject.ManifestDirectory, "template-manifest.json");
        }

        public void GetOrUpdateManifest()
        {
            Directory.Delete(CurrentProject.ManifestDirectory, true);
            Repository.Clone(ManifestRepo, CurrentProject.ManifestDirectory);
        }

        public List<LibraryTemplate> GetTemplates()
        {
            if (!File.Exists(ManifestFile))
            {
                GetOrUpdateManifest();
            }

            var manifestData = JsonConvert.DeserializeObject<TemplateManifest>(File.ReadAllText(ManifestFile));
            return manifestData.Templates;
        }

        public bool RemoveTemplateFromProject(string templateName)
        {
            CurrentProject.Templates.Remove(templateName);
            return true;
        }

        public bool AddTemplateToProject(LibraryTemplate template)
        {
            if (CurrentProject.Templates.Any(t => t == template.Name))
            {
                return false;
            }
            
            // it is at this point that we should clone the template into the templates folder, and
            // only add it to the project if that process is successful
            var templatePath = Path.Combine(CurrentProject.TemplateDirectory, template.Name);
            Repository.Clone(template.Repo, templatePath);
            //CustomizeTemplate(template.Name);

            CurrentProject.Templates.Add(template.Name);
            return true;
        }

        public LibraryTemplate GetTemplate(string templateName)
        {
            return GetTemplates().FirstOrDefault(t => t.Name == templateName);
        }

        public TemplateMeta GetTemplateMeta(string templateName)
        {
            var templates = GetTemplates();
            var template = templates.FirstOrDefault(t => t.Name == templateName);
            if (template != null && CurrentProject.Templates.Contains(templateName))
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
            foreach (var file in templateMeta.FilesToRename)
            {
                var fileToUpdate = Path.Combine(templatePath, file.OriginalName);
                if (File.Exists(fileToUpdate))
                {
                    var newFileName = file.NewName.Replace("%%NAME%%", CurrentProject.Name);
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
                );
            }
        }
    }
}