using System;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Objects;

namespace CLI.Commands.Template
{
    [Command("list", Description = "Get list of available templates")]
    public class TemplateListCommand
    {
        private int OnExecute(IConsole console)
        {
            try
            {
                var ps = new ProjectService();
                var project = ps.LoadProject(Environment.CurrentDirectory);
                var ts = new TemplateService(project);
                
                ts.RenderTemplateList();
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