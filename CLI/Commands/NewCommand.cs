using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services;

namespace CLI.Commands
{
    [Command("new", Description = "Create a new project")]
    public class NewCommand
    {
        [Required(ErrorMessage = "You must specify the project name")]
        [Argument(0, Description = "The name for the new project")]
        public string Name { get; }
        private int OnExecute(IConsole console)
        {
            console.WriteLine("Creating project: " + Name);
            var ps = new ProjectService();
            ps.CreateProject(Name, Environment.CurrentDirectory);
            return 0;
        }
    }
}