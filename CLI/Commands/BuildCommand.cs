using System;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services;

namespace CLI.Commands
{
    [Command("build", Description = "Build the client libraries")]
    public class BuildCommand
    {
        private int OnExecute(IConsole console)
        {
            var ps = new ProjectService();
            try
            {
                var project = ps.LoadProject(Environment.CurrentDirectory);
                console.WriteLine($"Building {project.Name}...");
                ps.BuildProject(project);
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