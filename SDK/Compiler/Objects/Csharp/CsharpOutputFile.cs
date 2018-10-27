using System.IO;
using Nebula.SDK.Objects;

namespace Nebula.SDK.Compiler.Objects.Csharp
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