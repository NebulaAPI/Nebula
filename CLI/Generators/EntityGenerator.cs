using System;
using System.Collections.Generic;
using System.IO;
using CLI.Models;
using Nebula.Models;
using Nebula.Parser;
using Newtonsoft.Json;
using SharpPad;

namespace Nebula.Generators
{
    public class EntityGenerator
    {
        public string Input { get; set; }

        private List<Entity> NewEntities { get; set; }

        public EntityGenerator(Project p, string input)
        {
            Input = input;
            NewEntities = new List<Entity>();
        }

        public void GenerateEntityFromJSON()
        {
            if (File.Exists(Input))
            {
                var stream = new InputStream(new FileInfo(Input));
                var tokens = new Tokenizer(stream);
                var parser = new JsonParser(tokens);
                var root = parser.Parse();
                
                FindAndPrompt(root, null);
                NewEntities.DumpBlocking();
            }
        }

        private string Prompt(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }

        private Entity FindAndPrompt(JsonObject obj, Entity parentEntity)
        {
            if (obj.IsObject)
            {
                var newEntity = new Entity();
                NewEntities.Add(newEntity);
                
                if (parentEntity == null)
                {
                    newEntity.Name = Prompt("Enter name for root entity: ");
                }
                else
                {
                    var name = Prompt($"Enter name of sub-object of {parentEntity.Name} and property {obj.Name}: ");
                    newEntity.Name = name;
                    parentEntity.Fields.Add(new EntityField() { Name = name, Type = "FIXME"});
                }

                foreach (var child in obj.Children)
                {
                    FindAndPrompt(child, newEntity);
                }
                return newEntity;
            }

            if (obj.IsValue && parentEntity != null)
            {
                parentEntity.Fields.Add(new EntityField() { Name = obj.Name, Type = "FIXME"});
                return parentEntity;
            }

            return null;
        }
    }
}