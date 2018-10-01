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
                switch (root)
                {
                    case JsonObject jsonObject:
                        FindAndPrompt(jsonObject);
                        break;
                }
                root.Dump();
            }
        }

        private void FindAndPrompt(JsonObject obj)
        {
            if (obj.Objects.Count > 0)
            {
                foreach (var key in obj.Objects.Keys)
                {
                    var valueName = "";
                    if (key == "")
                    {
                        valueName = "Object";
                    }
                    else
                    {
                        valueName = key;
                    }

                    var newEntity = new Entity();
                    Console.Write($"Enter name for root entity ({valueName}): ");
                    var name = Console.ReadLine();
                    newEntity.Name = name;
                    if (obj.Objects[key].Values.Count > 0)
                    {
                        
                    }
                }
            }
        }
    }
}