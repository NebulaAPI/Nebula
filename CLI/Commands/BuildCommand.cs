using System;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;

namespace CLI.Commands
{
    [Command("build", Description = "Build the client libraries")]
    public class BuildCommand
    {
        private IProjectService _projectService;

        public BuildCommand(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        private int OnExecute(IConsole console)
        {
            try
            {
                var project = _projectService.LoadProject(Environment.CurrentDirectory);
                console.WriteLine($"Building {project.Name}...");
                _projectService.BuildProject(project);
                console.WriteLine("Build completed successfully.");
                return 0;
            }
            catch (Exception e)
            {
                console.Error.WriteLine(e.Message);
                console.Error.WriteLine(e.StackTrace);
                return 1;
            }
        }
    }
}