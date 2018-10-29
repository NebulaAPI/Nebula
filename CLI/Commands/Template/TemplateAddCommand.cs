using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services;
using Nebula.SDK.Objects;

namespace CLI.Commands.Template
{
    [Command("add", Description = "Adds the specified template to the project")]
    public class TemplateAddCommand
    {
        [Required(ErrorMessage = "You must specify the template name")]
        [Argument(0, Description = "The name of the template to add")]
        public string Name { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var ps = new ProjectService();
                var project = ps.LoadProject(Environment.CurrentDirectory);
                var ts = new TemplateService(project, NebulaConfig.TemplateManifestRepo);
                
                var template = ts.GetTemplates().FirstOrDefault(t => t.Name == Name);
                if (template != null)
                {
                    if (ts.AddTemplateToProject(template))
                    {
                        ps.SaveProject(project);
                        return 0;
                    }

                    throw new Exception("Template is already added to this project.");
                }

                throw new Exception("Could not find template named: " + Name);
                
            }
            catch (Exception e)
            {
                console.Error.WriteLine(e.Message);
                return 1;
            }
        }
    }
}