using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Util;

namespace CLI.Commands.Template
{
    [Command("search", Description = "Search for templates")]
    public class TemplateSearchCommand
    {
        [Required(ErrorMessage = "You must specify the search query")]
        [Argument(0, Description = "The terms to search for")]
        public string Query { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var rs = new RegistryService();
                var templates = rs.SearchTemplates(Query);
                var table = new ConsoleTable("Name", "Description");
            
                foreach (var t in templates)
                {
                    table.AddRow(t.Name, t.Description);
                }
                table.Write(Format.Minimal);
                
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