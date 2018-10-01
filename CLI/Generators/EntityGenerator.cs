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
                
                root.DumpBlocking();
            }
        }

        
    }
}