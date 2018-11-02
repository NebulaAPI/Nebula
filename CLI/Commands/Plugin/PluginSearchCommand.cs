using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Util;

namespace CLI.Commands.Plugin
{
    [Command("search", Description = "Searches for the specified query")]
    public class PluginSearchCommand
    {
        private IRegistryService _registryService;
        private ConsoleTable _consoleTable;
        
        public PluginSearchCommand(IRegistryService registryService, ConsoleTable consoleTable)
        {
            _registryService = registryService;
            _consoleTable = consoleTable;
        }
        
        [Required(ErrorMessage = "You must specify a search term")]
        [Argument(0, Description = "The term(s) to search for")]
        public string Query { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var results = _registryService.SearchPlugins(Query);
                _consoleTable.AddColumn(new string[] { "Name", "Description"});
            
                foreach (var plugin in results)
                {
                    _consoleTable.AddRow(plugin.Name, plugin.Description);
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