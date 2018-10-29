using CLI.Commands.Plugin;
using McMaster.Extensions.CommandLineUtils;

namespace CLI.Commands
{
    [Command("plugin", Description = "Commands to manage installed plugins"),
        Subcommand("search", typeof(PluginSearchCommand)),
        Subcommand("install", typeof(PluginInstallCommand))
    ]
    public class PluginCommand
    {
        private int OnExecute(IConsole console)
        {
            return 0;
        }
    }
}