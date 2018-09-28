using System;
using System.Collections.Generic;

namespace Nebula.Compiler.Objects
{
    public abstract class OutputFile
    {
        public RootObject Root { get; set; }

        public List<string> RawContent { get; set; }

        public string FileName { get; set; }
        
        public OutputFile(RootObject root)
        {
            Root = root;
        }

        public string GetFileContent()
        {
            return string.Join(Environment.NewLine, RawContent) + Environment.NewLine;
        }

        protected abstract string GetFileExtension();
        
    }
}