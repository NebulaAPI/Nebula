using System.IO;
using LibGit2Sharp;
using Nebula.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Nebula.Parser;
using Nebula.Renderers;
using Nebula.Compiler;
using Nebula.Util;
using Core.Models;
using Core.Plugin;

namespace Core.Services
{
    public class ProjectService
    {
        private Project CurrentProject { get; set; }
        private int BuildProgress { get; set; }
        private int BuildCount { get; set; }
        private int TotalFiles { get; set; }
        
        /// <summary>
        /// Creates a new project
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <returns></returns>
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
            Repository.Clone(NebulaConfig.ProjectSkeletonRepo, repoPath);
            Directory.Delete(Path.Combine(repoPath, ".git"), true);
            Repository.Init(repoPath);
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
            var lines = File.ReadLines(projectFile);
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
            File.WriteAllLines(projectFile, outputLines);
        }

        public Project LoadProject(string location)
        {
            var projectFile = Path.Combine(location, "nebula.json");
            if (!File.Exists(projectFile))
            {
                throw new Exception("Could not find nebula.json in current directory");
            }
            var project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(projectFile));
            project.SourceDirectory = Path.Combine(location, "src");
            project.TemplateDirectory = Path.Combine(location, "templates");
            project.OutputDirectory = Path.Combine(location, "out");
            project.ManifestDirectory = Path.Combine(location, "manifest");
            project.ProjectDirectory = location;
            if (!Directory.Exists(project.SourceDirectory))
            {
                throw new Exception("Could not find src directory within project structure");
            }
            CurrentProject = project;
            return project;
        }

        public void SaveProject(Project project)
        {
            var projectFile = Path.Combine(project.ProjectDirectory, "nebula.json");
            var json = JsonConvert.SerializeObject(project);
            File.WriteAllText(projectFile, json);
        }

        public void BuildProject(Project p)
        {
            // first we need to build a list of files to be parsed, including their directory structure
            var files = new List<string>();
            GenerateFileList(p.SourceDirectory, files);
            TotalFiles = files.Count;
            BuildCount = 0;
            BuildProgress = 0;
            var modules = new List<ModuleNode>();
            
            // Build and validate the AST for the entire project
            var projectNode = new ProjectNode(files.Select(f => BuildModule(f)).ToList());
            var validator = new Validator(projectNode);
            validator.Validate();

            var ts = new TemplateService(p, null);

            foreach (var template in p.Templates)
            {
                var t = ts.GetTemplate(template) ?? throw new Exception("Could not find template data for template: " + template);
                
                var templateMeta = ts.GetTemplateMeta(template);

                var templatePluginFileFolder = Path.Combine(p.TemplateDirectory, t.Name, templateMeta.PluginLocation.Trim(Path.DirectorySeparatorChar));
                var pluginFiles = new List<string>();
                GenerateFileList(templatePluginFileFolder, pluginFiles, ".cs");

                var pluginService = new PluginService(pluginFiles.ToArray());
                var renderPlugin = pluginService.GetPlugin<IRenderPlugin>();
                var compilerPlugin = pluginService.GetPlugin<ICompilerPlugin>();

                var compiler = CompilerFactory.Get(t.Language, p, projectNode, templateMeta, compilerPlugin);
                var renderer = RendererFactory.Get(t.Language, compiler, renderPlugin);
                
                var destinationDirectory = PrepareOutputDir(p, templateMeta);
                renderer.Render(compiler.OutputFiles);
                foreach (var file in compiler.OutputFiles)
                {
                    var outputFileName = Path.Combine(destinationDirectory, file.FileName);
                    File.WriteAllText(outputFileName, file.GetFileContent());
                }
            }
        }

        private string PrepareOutputDir(Project project, TemplateMeta templateMeta)
        {
            // here we need to copy the template folder to the output directory
            // and customize the template
            var templateName = templateMeta.TemplateData.Name;
            var sourceTemplatePath = Path.Combine(project.TemplateDirectory, templateName, templateMeta.TemplateLocation.Trim(Path.DirectorySeparatorChar));
            var destTemplatePath = Path.Combine(project.OutputDirectory, $"{project.Name}-{templateName}");

            if (Directory.Exists(destTemplatePath))
            {
                Directory.Delete(destTemplatePath, true);
            }
            
            FileUtil.Copy(sourceTemplatePath, destTemplatePath);

            var ts = new TemplateService(project, null);
            ts.CustomizeTemplate(destTemplatePath, templateMeta.TemplateData.Name);
            return destTemplatePath;
        }

        private ModuleNode BuildModule(string inputFile)
        {
            BuildCount++;
            BuildProgress = (BuildCount / TotalFiles) * 100;
            Console.WriteLine($"[{BuildCount}/{TotalFiles} ({BuildProgress}%)] Processing {inputFile}");
            var moduleName = inputFile.Replace(Path.DirectorySeparatorChar, '.').Replace(".neb", "");
            var absoluteFile = Path.Combine(CurrentProject.SourceDirectory, inputFile);
            var parser = new NebulaParser(absoluteFile);
            return parser.Parse(moduleName);
        }

        private void GenerateFileList(string folder, List<string> allFiles, string ext = ".neb")
        {
            var filesInThisFolder = Directory
                .GetFiles(folder)
                .Select(f => f.Replace(CurrentProject.SourceDirectory + Path.DirectorySeparatorChar, ""))
                .Where(f => f.EndsWith(ext));
            allFiles.AddRange(filesInThisFolder);
            var folders = Directory.GetDirectories(folder);
            foreach (var f in folders)
            {
                GenerateFileList(f, allFiles);
            }
        }
    }
}