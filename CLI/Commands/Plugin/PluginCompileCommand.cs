using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;

namespace CLI.Commands.Plugin
{
    [Command("compile", Description = "Compile the specified plugin")]
    public class PluginCompileCommand
    {
        private IRegistryService _registryService;
        
        public PluginCompileCommand(IRegistryService registryService)
        {
            _registryService = registryService;
        }
        
        [Required(ErrorMessage = "You must specify a plugin to compile")]
        [Argument(0, Description = "The name of the plugin to compile")]
        public string Name { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                _registryService.Compile(Name);

                return 0;
            }
            catch (Exception e)
            {
                console.Error.WriteLine(e.Message);
                console.Error.WriteLine(e.StackTrace);
                return 1;
            }
        }
    }
}