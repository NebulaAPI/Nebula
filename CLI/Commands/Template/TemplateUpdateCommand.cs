using System;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Objects;

namespace CLI.Commands.Template
{
    [Command("update", Description = "Update template manifest")]
    public class TemplateUpdateCommand
    {
        private IProjectService _projectService;
        private ITemplateService _templateService;

        public TemplateUpdateCommand(IProjectService projectService, ITemplateService templateService)
        {
            _projectService = projectService;
            _templateService = templateService;
        }
        
        private int OnExecute(IConsole console)
        {
            console.WriteLine("Updating templates");
            console.WriteLine();
            try
            {
                var project = _projectService.LoadProject(Environment.CurrentDirectory);
                
                _templateService.CreateEmptyManifest();
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