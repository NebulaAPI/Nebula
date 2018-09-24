using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Nebula.Parser;
using McMaster.Extensions.CommandLineUtils;
using Nebula.Services;
using System.Linq;

namespace Nebula
{
    [Command(Name = "nebula", Description = "REST API Client Library Generator"),
        Subcommand("new", typeof(NewProject)), 
        Subcommand("build", typeof(BuildProject)), 
        Subcommand("template", typeof(TemplateOptions))
    ]
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

        [Command("template", Description = "Commands to manage library templates"),
            Subcommand("update", typeof(TemplateUpdateOption)),
            Subcommand("list", typeof(TemplateListOption)),
            Subcommand("add", typeof(TemplateAddOption)),
            Subcommand("remove", typeof(TemplateRemoveOption))
        ]
        private class TemplateOptions
        {
            private int OnExecute(IConsole console)
            {
                return 0;
            }

            [Command("update", Description = "Update template manifest")]
            private class TemplateUpdateOption
            {
                private int OnExecute(IConsole console)
                {
                    console.WriteLine("Updating templates");
                    try
                    {
                        var ps = new ProjectService();
                        var project = ps.LoadProject(Environment.CurrentDirectory);
                        var ts = new TemplateService(project, "https://github.com/JasonMiesionczek/Nebula-template-manifest.git");
                        
                        ts.GetOrUpdateManifest();
                        return 0;
                    }
                    catch (Exception e)
                    {
                        console.Error.WriteLine(e.Message);
                        return 1;
                    }
                    
                }
            }

            [Command("list", Description = "Get list of available templates")]
            private class TemplateListOption
            {
                private int OnExecute(IConsole console)
                {
                    try
                    {
                        var ps = new ProjectService();
                        var project = ps.LoadProject(Environment.CurrentDirectory);
                        var ts = new TemplateService(project, "https://github.com/JasonMiesionczek/Nebula-template-manifest.git");
                        
                        var templates = ts.GetTemplates();
                        foreach (var t in templates)
                        {
                            console.WriteLine($"{t.Name}\t{t.Language}\t{t.Framework}\t{t.Version}");
                        }
                        return 0;
                    }
                    catch (Exception e)
                    {
                        console.Error.WriteLine(e.Message);
                        return 1;
                    }
                }
            }

            [Command("add", Description = "Adds the specified template to the project")]
            private class TemplateAddOption
            {
                [Required(ErrorMessage = "You must specify the template name")]
                [Argument(0, Description = "The name of the template to add")]
                public string Name { get; }
                private int OnExecute(IConsole console)
                {
                    try
                    {
                        var ps = new ProjectService();
                        var project = ps.LoadProject(Environment.CurrentDirectory);
                        var ts = new TemplateService(project, "https://github.com/JasonMiesionczek/Nebula-template-manifest.git");
                        
                        var template = ts.GetTemplates().FirstOrDefault(t => t.Name == Name);
                        if (template != null)
                        {
                            if (ts.AddTemplateToProject(template))
                            {
                                ps.SaveProject(project);
                                return 0;
                            }

                            throw new Exception("Template is already added to this project.");
                        }

                        throw new Exception("Could not find template named: " + Name);
                        
                    }
                    catch (Exception e)
                    {
                        console.Error.WriteLine(e.Message);
                        return 1;
                    }
                }
            }

            [Command("remove", Description = "Removes the specified template from the project")]
            private class TemplateRemoveOption
            {
                [Required(ErrorMessage = "You must specify the template name")]
                [Argument(0, Description = "The name of the template to remove")]
                public string Name { get; }
                private int OnExecute(IConsole console)
                {
                    try
                    {
                        var ps = new ProjectService();
                        var project = ps.LoadProject(Environment.CurrentDirectory);
                        var ts = new TemplateService(project, "https://github.com/JasonMiesionczek/Nebula-template-manifest.git");
                        
                        ts.RemoveTemplateFromProject(Name);
                        ps.SaveProject(project);
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
}
