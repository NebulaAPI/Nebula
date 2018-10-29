using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CLI.Commands.Template;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Core.Services;
using Nebula.SDK.Objects;

namespace CLI.Commands
{
    [Command("template", Description = "Commands to manage library templates"),
        Subcommand("update", typeof(TemplateUpdateCommand)),
        Subcommand("list", typeof(TemplateListCommand)),
        Subcommand("add", typeof(TemplateAddCommand)),
        Subcommand("remove", typeof(TemplateRemoveCommand))
    ]
    public class TemplateCommand
    {
        private int OnExecute(IConsole console)
        {
            return 0;
        }
    }
}