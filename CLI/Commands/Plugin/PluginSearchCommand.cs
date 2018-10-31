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
        [Required(ErrorMessage = "You must specify a search term")]
        [Argument(0, Description = "The term(s) to search for")]
        public string Query { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var registryService = new RegistryService();
                var results = registryService.SearchPlugins(Query);
                var table = new ConsoleTable("Name", "Descsription");
            
                foreach (var plugin in results)
                {
                    table.AddRow(plugin.Name, plugin.Description);
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