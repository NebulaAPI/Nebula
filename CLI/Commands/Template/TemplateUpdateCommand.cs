using System;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Objects;

namespace CLI.Commands.Template
{
    [Command("update", Description = "Update template manifest")]
    public class TemplateUpdateCommand
    {
        private int OnExecute(IConsole console)
        {
            console.WriteLine("Updating templates");
            console.WriteLine();
            try
            {
                var ps = new ProjectService();
                var project = ps.LoadProject(Environment.CurrentDirectory);
                var ts = new TemplateService(project, NebulaConfig.TemplateManifestRepo);
                
                ts.GetOrUpdateManifest();
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