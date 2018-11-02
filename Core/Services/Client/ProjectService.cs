using System.IO;
using LibGit2Sharp;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Nebula.SDK.Objects;
using Nebula.Core.Parser;
using Nebula.SDK.Plugin;
using Nebula.SDK.Compiler;
using Nebula.SDK.Renderers;
using Nebula.SDK.Util;
using Nebula.Core.Services.Client;
using Nebula.SDK.Interfaces;
using Nebula.Core.Factories;
using Nebula.SDK.Objects.Client;

namespace Nebula.Core.Services.Client
{
    public interface IProjectService
    {
        /// <summary>
        /// Creates a new project
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        Project CreateProject(string name, string location);
        Project LoadProject(string location);
        void SaveProject(Project project);
        void AddTemplate(Project project, Template template);
        void BuildProject(Project p);
    }
    
    public class ProjectService : IProjectService
    {
        private Project _currentProject;
        private double _buildProgress;
        private double _buildCount;
        private double _totalFiles;

        private IRegistryService _registryService;
        private ITemplateService _templateService;
        private IFileUtil _fileUtil;
        private IGitService _gitService;
        private ICompilerFactory _compilerFactory;
        private IProjectValidator _projectValidator;
        
        public ProjectService(
            IRegistryService registryService,
            ITemplateService templateService,
            IFileUtil fileUtil,
            IGitService gitService,
            ICompilerFactory compilerFactory,
            IProjectValidator projectValidator
        ) {
            _registryService = registryService;
            _templateService = templateService;
            _fileUtil = fileUtil;
            _gitService = gitService;
            _compilerFactory = compilerFactory;
            _projectValidator = projectValidator;
        }
        
        public Project CreateProject(string name, string location)
        {
            var p = new Project 
            { 
                Name = name,
                Version = "1.0.0", // FIXME: set version
                Author = Environment.UserName,
                Company = "Some Company"  // FIXME: set company name
            };
            var repoPath = Path.Combine(location, name);

            // TODO: make this path a config setting
            _gitService.Clone(NebulaConfig.ProjectSkeletonRepo, repoPath);
            _fileUtil.DirectoryDelete(Path.Combine(repoPath, ".git"), true);
            _gitService.Init(repoPath);
            UpdateProjectConfig(p, repoPath);

            return p;
        }

        /// <summary>
        /// Replaces the temporary variables inside the nebula.json file
        /// </summary>
        /// <param name="p"></param>
        /// <param name="projectPath"></param>
        private void UpdateProjectConfig(Project p, string projectPath)
        {
            var variableMap = new Dictionary<string, Func<string>>()
            {
                { "NAME", () => p.Name },
                { "VERSION", () => p.Version },
                { "AUTHOR", () => p.Author },
                { "COMPANY", () => p.Company }
                // TODO: add other variables
            };
            
            var projectFile = Path.Combine(projectPath, "nebula.json");
            var lines = _fileUtil.FileReadLines(projectFile);
            var regex = new Regex("%%(.*)%%");
            var outputLines = new List<string>();
            foreach (var line in lines)
            {
                var matches = regex.Matches(line);
                if (matches.Count > 0)
                {
                    foreach (Match match in matches)
                    {
                        foreach (Group group in match.Groups)
                        {
                            var name = group.Value;
                            if (variableMap.ContainsKey(name))
                            {
                                var newLine = line.Replace($"%%{name}%%", variableMap[name]());
                                outputLines.Add(newLine);
                            }
                        }
                    }
                }
                else
                {
                    outputLines.Add(line);
                }
            }

            _fileUtil.FileWriteAllLines(projectFile, outputLines);
        }

        public Project LoadProject(string location)
        {
            var projectFile = Path.Combine(location, "nebula.json");
            if (!_fileUtil.FileExists(projectFile))
            {
                throw new Exception("Could not find nebula.json in current directory");
            }
            var project = JsonConvert.DeserializeObject<Project>(_fileUtil.FileReadAllText(projectFile));
            project.SourceDirectory = Path.Combine(location, "src");
            project.TemplateDirectory = Path.Combine(location, "templates");
            project.OutputDirectory = Path.Combine(location, "out");
            project.ManifestDirectory = Path.Combine(location, "manifest");
            project.ProjectDirectory = location;
            if (!_fileUtil.DirectoryExists(project.SourceDirectory))
            {
                throw new Exception("Could not find src directory within project structure");
            }
            _currentProject = project;
            return project;
        }

        public void SaveProject(Project project)
        {
            var projectFile = Path.Combine(project.ProjectDirectory, "nebula.json");
            var json = JsonConvert.SerializeObject(project);
            _fileUtil.FileWriteAllText(projectFile, json);
        }

        public void AddTemplate(Project project, Template template)
        {
            project.Templates.Add(template.Name, template.Versions.FirstOrDefault()?.Version); // FIXME: figure out the most recent version
            SaveProject(project);
        }

        public void BuildProject(Project p)
        {
            // first we need to build a list of files to be parsed, including their directory structure
            var files = new List<string>();
            _fileUtil.GenerateFileList(p.SourceDirectory, files, ".neb", (f) => f.Replace(_currentProject.SourceDirectory + Path.DirectorySeparatorChar, ""));
            _totalFiles = files.Count;
            _buildCount = 0.0f;
            _buildProgress = 0.0f;
            var modules = new List<ModuleNode>();

            if (_totalFiles == 0)
            {
                throw new Exception("No .neb files found. Nothing to build.");
            }
            
            // Build and validate the AST for the entire project
            var projectNode = new ProjectNode(files.Select(f => BuildModule(f)).ToList());
            _projectValidator.Validate(projectNode);

            //var cf = new CompilerFactory(_registryService);

            // foreach (var template in p.Templates)
            // {
            //     var t = ts.GetTemplate(template) ?? throw new Exception("Could not find template data for template: " + template);
                
            //     var templateMeta = ts.GetTemplateMeta(template);

            //     var templatePluginFileFolder = Path.Combine(p.TemplateDirectory, t.Name, templateMeta.PluginLocation.Trim(Path.DirectorySeparatorChar));
            //     var pluginFiles = new List<string>();
            //     FileUtil.GenerateFileList(templatePluginFileFolder, pluginFiles, ".cs", (f) => f.Replace(CurrentProject.SourceDirectory + Path.DirectorySeparatorChar, ""));

            //     var pluginService = new PluginService("");
            //     var renderPlugin = pluginService.GetPlugin<IRenderPlugin>();
            //     var compilerPlugin = pluginService.GetPlugin<ICompilerPlugin>();

            //     var compiler = CompilerFactory.Get(t.Language);
            //     compiler.Init(p, projectNode, templateMeta, compilerPlugin);
            //     var outputFiles = compiler.Compile();
            //     var renderer = RendererFactory.Get(t.Language, compiler, renderPlugin);
                
            //     var destinationDirectory = PrepareOutputDir(p, templateMeta);
            //     renderer.Render(outputFiles, p, templateMeta);
            //     foreach (var file in outputFiles)
            //     {
            //         var outputFileName = Path.Combine(destinationDirectory, templateMeta.SourceFolder, file.FileName);
            //         File.WriteAllText(outputFileName, file.GetFileContent());
            //     }
            // }
        }

        private string PrepareOutputDir(Project project, TemplateMeta templateMeta)
        {
            // here we need to copy the template folder to the output directory
            // and customize the template
            var templateName = templateMeta.TemplateData.Name;
            var sourceTemplatePath = Path.Combine(project.TemplateDirectory, templateName, templateMeta.TemplateLocation.Trim(Path.DirectorySeparatorChar));
            var destTemplatePath = Path.Combine(project.OutputDirectory, $"{project.Name}-{templateName}");

            if (_fileUtil.DirectoryExists(destTemplatePath))
            {
                _fileUtil.DirectoryDelete(destTemplatePath, true);
            }
            
            _fileUtil.Copy(sourceTemplatePath, destTemplatePath);

            _templateService.CustomizeTemplate(project, destTemplatePath, templateMeta.TemplateData.Name);

            return destTemplatePath;
        }

        private ModuleNode BuildModule(string inputFile)
        {
            _buildCount++;
            _buildProgress = Math.Floor((_buildCount / _totalFiles) * 100);
            Console.WriteLine($"[{_buildCount}/{_totalFiles} ({_buildProgress}%)] Processing {inputFile}");
            var moduleName = inputFile.Replace(Path.DirectorySeparatorChar, '.').Replace(".neb", "");
            var absoluteFile = Path.Combine(_currentProject.SourceDirectory, inputFile);
            var parser = new NebulaParser(absoluteFile);

            return parser.Parse(moduleName);
        }
    }
}