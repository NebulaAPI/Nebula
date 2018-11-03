using System;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Objects;

namespace CLI.Commands.Template
{
    [Command("list", Description = "Get list of available templates")]
    public class TemplateListCommand
    {
        private IProjectService _projectService;
        private ITemplateService _templateService;

        public TemplateListCommand(IProjectService projectService, ITemplateService templateService)
        {
            _projectService = projectService;
            _templateService = templateService;
        }
        
        private int OnExecute(IConsole console)
        {
            try
            {
                var project = _projectService.LoadProject(Environment.CurrentDirectory);
                
                _templateService.RenderTemplateList(project);
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