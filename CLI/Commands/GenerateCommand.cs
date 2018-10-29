using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Generators;
using Nebula.Core.Services;

namespace CLI.Commands
{
    [Command("generate", Description = "Tools for generating entities"),
        Subcommand("entity", typeof(GenerateEntityOption))]
    public class GenerateCommand
    {
        private int OnExecute(IConsole console)
        {
            return 0;
        }

        [Command("entity", Description = "Generate entities from provided JSON")]
        private class GenerateEntityOption
        {
            [Required(ErrorMessage = "You must specify the source data")]
            [Argument(0, Description = "JSON content from which to generate entities")]
            public string Data { get; }
            private int OnExecute(IConsole console)
            {
                var ps = new ProjectService();
                try
                {
                    var project = ps.LoadProject(Environment.CurrentDirectory);
                    var generator = new EntityGenerator(project, Data);
                    generator.GenerateEntityFromJSON();
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
}