using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services;
using Nebula.SDK.Objects;

namespace CLI.Commands.Template
{
    [Command("remove", Description = "Removes the specified template from the project")]
    public class TemplateRemoveCommand
    {
        [Required(ErrorMessage = "You must specify the template name")]
        [Argument(0, Description = "The name of the template to remove")]
        public string Name { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var ps = new ProjectService();
                var project = ps.LoadProject(Environment.CurrentDirectory);
                var ts = new TemplateService(project, NebulaConfig.TemplateManifestRepo);
                
                ts.RemoveTemplateFromProject(Name);
                ps.SaveProject(project);
                return 0;
            }
            catch (Exception e)
            {
                console.Error.WriteLine(e.Message);
                return 1;
            }
        }
    }
}