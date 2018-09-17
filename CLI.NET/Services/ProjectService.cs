using System.IO;
using LibGit2Sharp;
using Nebula.Models;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using CLI.NET.Parser;

namespace Nebula.Services
{
    public class ProjectService
    {
        private Project CurrentProject { get; set; }
        
        /// <summary>
        /// Creates a new project
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public Project CreateProject(string name, string location)
        {
            var p = new Project { Name = name };
            var repoPath = Path.Join(location, name);

            // TODO: make this path a config setting
            Repository.Clone("https://github.com/JasonMiesionczek/Nebula-project-skeleton.git", repoPath);
            Directory.Delete(Path.Join(repoPath, ".git"), true);
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
                { "NAME", () => p.Name }
                // TODO: add other variables
            };
            
            var projectFile = Path.Join(projectPath, "nebula.json");
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
            var projectFile = Path.Join(location, "nebula.json");
            if (!File.Exists(projectFile))
            {
                throw new Exception("Could not find nebula.json in current directory");
            }
            var project = JsonConvert.DeserializeObject<Project>(File.ReadAllText(projectFile));
            project.SourceDirectory = Path.Join(location, "src");
            if (!Directory.Exists(project.SourceDirectory))
            {
                throw new Exception("Could not find src directory within project structure");
            }
            CurrentProject = project;
            return project;
        }

        public void BuildProject(Project p)
        {
            // first we need to build a list of files to be parsed, including their directory structure
            var files = new List<string>();
            GenerateFileList(p.SourceDirectory, files);
            var modules = new List<ModuleNode>();
            
            var projectNode = new ProjectNode(files.Select(f => BuildModule(f)).ToList());
            var validator = new Validator(projectNode);
            validator.Validate();
        }

        private ModuleNode BuildModule(string inputFile)
        {
            var moduleName = inputFile.Replace(Path.DirectorySeparatorChar, '.').Replace(".neb", "");
            var absoluteFile = Path.Join(CurrentProject.SourceDirectory, Path.DirectorySeparatorChar.ToString(), inputFile);
            var parser = new Parser(absoluteFile);
            return parser.Parse(moduleName);
        }

        private void GenerateFileList(string folder, List<string> allFiles)
        {
            var filesInThisFolder = Directory
                .GetFiles(folder)
                .Select(f => f.Replace(CurrentProject.SourceDirectory + Path.DirectorySeparatorChar, ""))
                .Where(f => f.EndsWith(".neb"));
            allFiles.AddRange(filesInThisFolder);
            var folders = Directory.GetDirectories(folder);
            foreach (var f in folders)
            {
                GenerateFileList(f, allFiles);
            }
        }
    }
}