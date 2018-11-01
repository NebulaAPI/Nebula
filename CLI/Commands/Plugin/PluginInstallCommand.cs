using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using LibGit2Sharp;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services.Client;
using Nebula.SDK.Objects;

namespace CLI.Commands.Plugin
{
    [Command("install", Description = "Install the specified plugin")]
    public class PluginInstallCommand
    {
        [Required(ErrorMessage = "You must specify a plugin to install")]
        [Argument(0, Description = "The name of the plugin to install")]
        public string Name { get; }
        private int OnExecute(IConsole console)
        {
            try
            {
                var registryService = new RegistryService();
                registryService.InstallPlugin(Name);

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