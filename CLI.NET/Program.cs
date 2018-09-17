using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using CLI.NET.Parser;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Services;

namespace Nebula
{
    [Command(Name = "nebula", Description = "REST API Client Library Generator"),
    Subcommand("new", typeof(NewProject)), Subcommand("build", typeof(BuildProject))]
    class Nebula
    {
        public static void Main(string[] args) => CommandLineApplication.Execute<Nebula>(args);
        // public static void Main(string[] args)
        // {
        //     var sampleFile = File.ReadAllText("sample.neb");
        //     var stream = new InputStream(sampleFile);
        //     var tokenizer = new Tokenizer(stream);
        //     var parser = new Parser(tokenizer);
        //     var module = parser.Parse("test");

        //     var sampleEntity = File.ReadAllText("sample_entity.neb");
        //     var entityStream = new InputStream(sampleEntity);
        //     var entityTokenizer = new Tokenizer(entityStream);
        //     var entityParser = new Parser(entityTokenizer);
        //     var entityModule = entityParser.Parse("testEntity");

        //     while (!tokenizer.Eof())
        //     {
        //         var token = tokenizer.Next();
        //         Console.WriteLine(token);
        //     }
        // }

        private int OnExecute(CommandLineApplication app, IConsole console)
        {
            console.WriteLine("You must specify a subcommand.");
            app.ShowHelp();
            return 1;
        }

        [Command("new", Description = "Create a new project")]
        private class NewProject
        {
            [Required(ErrorMessage = "You must specify the project name")]
            [Argument(0, Description = "The name for the new project")]
            public string Name { get; }
            private int OnExecute(IConsole console)
            {
                console.WriteLine("Creating project: " + Name);
                var ps = new ProjectService();
                ps.CreateProject(Name, Environment.CurrentDirectory);
                return 0;
            }
        }

        [Command("build", Description = "Build the client libraries")]
        private class BuildProject
        {
            private int OnExecute(IConsole console)
            {
                var ps = new ProjectService();
                try
                {
                    var project = ps.LoadProject(Environment.CurrentDirectory);
                    console.WriteLine($"Building {project.Name}...");
                    ps.BuildProject(project);
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
}
