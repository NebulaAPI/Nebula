using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Objects;

namespace CLI.Commands.Template
{
    [Command("add", Description = "Adds the specified template to the project")]
    public class TemplateAddCommand
    {
        private IProjectService _projectService;
        private IRegistryService _registryService;

        public TemplateAddCommand(IProjectService projectService, IRegistryService registryService)
        {
            _projectService = projectService;
            _registryService = registryService;
        }
        
        [Required(ErrorMessage = "You must specify the template name")]
        [Argument(0, Description = "The name of the template to add")]
        public string Name { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var project = _projectService.LoadProject(Environment.CurrentDirectory);
                
                // clone the template into the cache folder
                var template = _registryService.InstallTemplate(Name);
                _projectService.AddTemplate(project, template);

                return 0;
                // var ts = new TemplateService(project);
                
                // var template = ts.GetTemplates().FirstOrDefault(t => t.Name == Name);
                // if (template != null)
                // {
                //     if (ts.AddTemplateToProject(template))
                //     {
                //         ps.SaveProject(project);
                //         return 0;
                //     }

                //     throw new Exception("Template is already added to this project.");
                // }

                // throw new Exception("Could not find template named: " + Name);
                
            }
            catch (Exception e)
            {
                console.Error.WriteLine(e.Message);
                return 1;
            }
        }
    }
}