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
using Nebula.Core.Helpers;

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
        private IRendererFactory _rendererFactory;
        private IProjectValidator _projectValidator;
        private ICompilationService _compilationService;
        
        public ProjectService(
            IRegistryService registryService,
            ITemplateService templateService,
            IFileUtil fileUtil,
            IGitService gitService,
            ICompilerFactory compilerFactory,
            IRendererFactory rendererFactory,
            IProjectValidator projectValidator,
            ICompilationService compilationService
        ) {
            _registryService = registryService;
            _templateService = templateService;
            _fileUtil = fileUtil;
            _gitService = gitService;
            _compilerFactory = compilerFactory;
            _rendererFactory = rendererFactory;
            _projectValidator = projectValidator;
            _compilationService = compilationService;
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
            project.OutputDirectory = Path.Combine(location, "out");
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
            if (project.Templates.ContainsKey(template.Name))
            {
                return;
            }
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

            foreach (var templateName in p.Templates.Keys)
            {
                var templateMeta = _templateService.GetTemplateMeta(p, templateName);
                var templateExtensionFolder = 
                    Path.Combine(
                        NebulaConfig.TemplateDirectory, 
                        templateMeta.TemplateData.Name,
                        templateMeta.Configuration.ExtensionLocation.Trim(Path.DirectorySeparatorChar)
                    );

                var extensionFiles = new List<string>();
                _fileUtil.GenerateFileList(templateExtensionFolder, extensionFiles, ".cs", (f) => f);
                var templateExtension = _compilationService.CompileInMemory(templateName, extensionFiles.ToArray());
                var compilerExtension = templateExtension.SearchForType<ICompilerExtension>();
                var rendererExtension = templateExtension.SearchForType<IRendererExtension>();

                var compiler = _compilerFactory.Get(templateMeta.LanguagePlugin);
                compiler.Init(p, projectNode, templateMeta, compilerExtension);
                var renderer = _rendererFactory.Get(templateMeta.LanguagePlugin, compiler, rendererExtension);
                var outputFiles = compiler.Compile();

                var destinationDirectory = PrepareOutputDir(p, templateMeta);
                renderer.Render(outputFiles, p, templateMeta);
                foreach (var file in outputFiles)
                {
                    var outputFileName = Path.Combine(destinationDirectory, templateMeta.Configuration.SourceFolder, file.FileName);
                    _fileUtil.FileWriteAllText(outputFileName, file.GetFileContent());
                }
            }
        }

        private string PrepareOutputDir(Project project, TemplateMeta templateMeta)
        {
            // here we need to copy the template folder to the output directory
            // and customize the template
            var templateName = templateMeta.TemplateData.Name;
            var sourceTemplatePath = Path.Combine(NebulaConfig.TemplateDirectory, templateName, templateMeta.Configuration.TemplateLocation.Trim(Path.DirectorySeparatorChar));
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