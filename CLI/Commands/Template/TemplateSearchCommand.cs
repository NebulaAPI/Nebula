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
        private IRegistryService _registryService;
        private ConsoleTable _consoleTable;
        
        public TemplateSearchCommand(IRegistryService registryService, ConsoleTable consoleTable)
        {
            _registryService = registryService;
            _consoleTable = consoleTable;
        }
        
        [Required(ErrorMessage = "You must specify the search query")]
        [Argument(0, Description = "The terms to search for")]
        public string Query { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var templates = _registryService.SearchTemplates(Query);
                _consoleTable.AddColumn(new string[] { "Name", "Description"});
            
                foreach (var t in templates)
                {
                    _consoleTable.AddRow(t.Name, t.Description);
                }
                _consoleTable.Write(Format.Minimal);
                
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