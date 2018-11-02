using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Nebula.SDK.Objects;
using Nebula.Core.Services;
using Nebula.Core.Generators;
using static System.Environment;
using System.Collections.Generic;
using Nebula.Core.Services.API;
using CLI.Commands;
using Nebula.Core.Services.Client;
using Nebula.SDK.Compiler.Abstracts;
using Microsoft.Extensions.DependencyInjection;
using Nebula.SDK.Util;
using Nebula.Core.Factories;
using Nebula.Core.Parser;

namespace Nebula
{
    [Command(Name = "nebula", Description = "REST API Client Library Generator"),
        Subcommand("new", typeof(NewCommand)), 
        Subcommand("build", typeof(BuildCommand)), 
        Subcommand("template", typeof(TemplateCommand)),
        Subcommand("generate", typeof(GenerateCommand)),
        Subcommand("plugin", typeof(PluginCommand))
    ]
    class Nebula
    {
        public static void Main(string[] args)
        {
            var appDataFolder = Environment.GetFolderPath(SpecialFolder.LocalApplicationData);
            PrepLocalDirectories(appDataFolder);
            Console.WriteLine($"Configuration location: {appDataFolder}");
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(NebulaConfig.ConfigurationDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            
            NebulaConfig.TemplateManifestRepo = configuration.GetSection("TemplateManifest").Value;
            NebulaConfig.ProjectSkeletonRepo = configuration.GetSection("ProjectSkeleton").Value;
            
            var app = new CommandLineApplication<Nebula>();

            var services = new ServiceCollection()
                .AddTransient<IRegistryService, RegistryService>()
                .AddTransient<IProjectService, ProjectService>()
                .AddTransient<ITemplateService, TemplateService>()
                .AddTransient<ICompilationService, CompilationService>()
                .AddSingleton<IProjectValidator, ProjectValidator>()
                .AddSingleton<IFileUtil, FileUtil>()
                .AddSingleton<IGitService, GitService>()
                .AddSingleton<ICompilerFactory, CompilerFactory>()
                .AddTransient(typeof(RegistryApiClient))
                .AddTransient(typeof(ConsoleTable))
                .BuildServiceProvider();
            
            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);
                
            app.Execute(args);
        }

        private static void PrepLocalDirectories(string appDataFolder)
        {
            var rootFolder = Path.Combine(appDataFolder, "nebula");
            var pluginFolder = Path.Combine(rootFolder, "plugins");
            var templateFolder = Path.Combine(rootFolder, "templates");

            var folders = new List<string> { pluginFolder, templateFolder };
            folders.ForEach(f => Directory.CreateDirectory(f));

            NebulaConfig.PluginDirectory = pluginFolder;
            NebulaConfig.TemplateDirectory = templateFolder;
            NebulaConfig.ConfigurationDirectory = rootFolder;
        }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("You must specify a subcommand.");
            app.ShowHelp();
            return 1;
        }
    }
}
