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
    public interface ITemplateService
    {
        Template InstallTemplate(string name);
        TemplateManifest LoadLocalManifest();
        void SaveLocalManifest(TemplateManifest manifest);
        void CreateEmptyManifest();
        List<Template> GetLocalTemplates();
        void RenderTemplateList(Project project);
        Template GetTemplate(string templateName);
        TemplateMeta GetTemplateMeta(Project project, string templateName);
        void CustomizeTemplate(Project project, string templatePath, string templateName);
    }
    
    public class TemplateService : ITemplateService
    {
        private string _manifestRepo;
        private string _manifestFile;
        private IRegistryService _registryService;
        private ICompilationService _compilationService;
        private IFileUtil _fileUtil;
        private ConsoleTable _consoleTable;
        
        public TemplateService(
            IRegistryService registryService,
            ICompilationService compilationService,
            IFileUtil fileUtil,
            ConsoleTable consoleTable
        ) {
            _manifestRepo = NebulaConfig.TemplateManifestRepo;
            _manifestFile = Path.Combine(NebulaConfig.TemplateDirectory, "template-manifest.json");
            _registryService = registryService;
            _compilationService = compilationService;
            _fileUtil = fileUtil;
            _consoleTable = consoleTable;
        }

        public Template InstallTemplate(string name)
        {
            var manifestData = LoadLocalManifest();
            var template = _registryService.InstallTemplate(name);
            manifestData.Templates.Add(template);
            SaveLocalManifest(manifestData);

            return template;
        }

        public TemplateManifest LoadLocalManifest()
        {
            if (!_fileUtil.FileExists(_manifestFile))
            {
                CreateEmptyManifest();
            }
            return JsonConvert.DeserializeObject<TemplateManifest>(_fileUtil.FileReadAllText(_manifestFile));
        }

        public void SaveLocalManifest(TemplateManifest manifest)
        {
            _fileUtil.FileWriteAllText(_manifestFile, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        public void CreateEmptyManifest()
        {
            var manifest = new TemplateManifest();
            _fileUtil.FileWriteAllText(_manifestFile, JsonConvert.SerializeObject(manifest, Formatting.Indented));
        }

        public List<Template> GetLocalTemplates()
        {
            if (!_fileUtil.FileExists(_manifestFile))
            {
                CreateEmptyManifest();
            }

            var manifestData = JsonConvert.DeserializeObject<TemplateManifest>(_fileUtil.FileReadAllText(_manifestFile));
            return manifestData.Templates;
        }

        public void RenderTemplateList(Project project)
        {
            var templates = GetLocalTemplates();
            //var table = new ConsoleTable("Name", "Description", "Added");
            _consoleTable.AddColumn(new string[] { "Name", "Description", "Added"});
            
            foreach (var t in templates)
            {
                var includedInProject = project.Templates.Keys.Contains(t.Name);
                _consoleTable.AddRow(t.Name, t.Description, includedInProject ? '\u2714': ' ');
            }
            _consoleTable.Write(Format.Minimal);
        }

        public Template GetTemplate(string templateName)
        {
            return GetLocalTemplates().FirstOrDefault(t => t.Name == templateName);
        }

        public TemplateMeta GetTemplateMeta(Project project, string templateName)
        {
            var templates = GetLocalTemplates();
            var template = templates.FirstOrDefault(t => t.Name == templateName);
            if (template != null && project.Templates.Keys.Contains(templateName))
            {
                var templatePath = Path.Combine(project.TemplateDirectory, templateName);
                if (!_fileUtil.DirectoryExists(templatePath))
                {
                    throw new System.Exception("Could not find template directory: " + templatePath);
                }
                var templateConfigFile = Path.Combine(templatePath, "nebula-meta.json");
                if (!_fileUtil.FileExists(templateConfigFile))
                {
                    throw new System.Exception("Could not find template meta file: " + templateConfigFile);
                }
                var templateMeta = JsonConvert.DeserializeObject<TemplateMeta>(_fileUtil.FileReadAllText(templateConfigFile));
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

        public void CustomizeTemplate(Project project, string templatePath, string templateName)
        {
            var templateMeta = GetTemplateMeta(project, templateName);
            foreach (var file in templateMeta.Configuration.FilesToRename.Keys)
            {
                var fileToUpdate = Path.Combine(templatePath, file);
                if (_fileUtil.FileExists(fileToUpdate))
                {
                    var newFileName = templateMeta.Configuration.FilesToRename[file].Replace("%%NAME%%", project.Name);
                    _fileUtil.FileMove(fileToUpdate, Path.Combine(templatePath, newFileName));
                }
            }

            foreach (var f in _fileUtil.DirectoryGetFiles(templatePath))
            {
                var fileContent = _fileUtil.FileReadAllText(f);
                _fileUtil.FileWriteAllText(f, 
                    fileContent
                        .Replace("%%NAME%%", project.Name)
                        .Replace("%%VERSION%%", project.Version)
                        .Replace("%%PROPERNAME%%", project.GetProperName())
                );
            }
        }
    }
}