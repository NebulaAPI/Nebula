using System.IO;
using Nebula.Core.Models;

namespace Nebula.Core.Compiler.Objects.Csharp
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