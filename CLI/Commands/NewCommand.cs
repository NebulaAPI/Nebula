using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;

namespace CLI.Commands
{
    [Command("new", Description = "Create a new project")]
    public class NewCommand
    {
        private IProjectService _projectService;

        public NewCommand(IProjectService projectService)
        {
            _projectService = projectService;
        }
        
        [Required(ErrorMessage = "You must specify the project name")]
        [Argument(0, Description = "The name for the new project")]
        public string Name { get; }
        private int OnExecute(IConsole console)
        {
            console.WriteLine("Creating project: " + Name);
            _projectService.CreateProject(Name, Environment.CurrentDirectory);
            return 0;
        }
    }
}