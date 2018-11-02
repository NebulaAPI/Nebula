using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Objects;

namespace CLI.Commands.Template
{
    [Command("remove", Description = "Removes the specified template from the project")]
    public class TemplateRemoveCommand
    {
        private IProjectService _projectService;
        private ITemplateService _templateService;

        public TemplateRemoveCommand(IProjectService projectService, ITemplateService templateService)
        {
            _projectService = projectService;
            _templateService = templateService;
        }
        
        [Required(ErrorMessage = "You must specify the template name")]
        [Argument(0, Description = "The name of the template to remove")]
        public string Name { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var project = _projectService.LoadProject(Environment.CurrentDirectory);
                
                //ts.RemoveTemplateFromProject(Name);
                _projectService.SaveProject(project);
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