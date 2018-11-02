using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Generators;
using Nebula.Core.Services.Client;
using Nebula.SDK.Util;

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
            private IProjectService _projectService;
            private IFileUtil _fileUtil;

            public GenerateEntityOption(IProjectService projectService, IFileUtil fileUtil)
            {
                _projectService = projectService;
                _fileUtil = fileUtil;
            }
            
            [Required(ErrorMessage = "You must specify the source data")]
            [Argument(0, Description = "JSON content from which to generate entities")]
            public string Data { get; }
            private int OnExecute(IConsole console)
            {
                try
                {
                    var project = _projectService.LoadProject(Environment.CurrentDirectory);
                    var generator = new EntityGenerator(_fileUtil);
                    generator.GenerateEntityFromJSON(project, Data);
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