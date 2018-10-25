using System.IO;
using Nebula.Models;
using Nebula.Parser;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpOutputFile : OutputFile
    {
        public CsharpOutputFile(RootObject root, TemplateMeta meta) : base(root, meta)
        {
            
        }

        protected override string GetFileExtension()
        {
            return "cs";
        }
    }
}