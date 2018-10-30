using System;
using System.Collections.Generic;
using System.IO;
using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Objects;

namespace Nebula.SDK.Compiler.Objects
{
    public abstract class OutputFile
    {
        public RootObject Root { get; set; }

        public TemplateMeta Meta { get; set; }

        public List<string> RawContent { get; set; }

        public string FileName { get; set; }
        
        public OutputFile(RootObject root, TemplateMeta meta)
        {
            Root = root;
            Meta = meta;

            switch (root)
            {
                case AbstractClass<EntityNode> e:
                    FileName = Path.Combine(meta.Configuration.EntityLocation, $"{e.Name}.{GetFileExtension()}");
                    break;
                case AbstractClass<ApiNode> c:
                    FileName = Path.Combine(meta.Configuration.ClientLocation, $"{c.Name}Client.{GetFileExtension()}");
                    break;
            }
        }

        public string GetFileContent()
        {
            return string.Join(Environment.NewLine, RawContent) + Environment.NewLine;
        }

        protected abstract string GetFileExtension();
        
    }
}