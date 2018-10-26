using System.IO;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Objects.PHP
{
    public class PhpOutputFile : OutputFile
    {
        public PhpOutputFile(RootObject root, TemplateMeta meta) : base(root, meta)
        {
            
        }

        protected override string GetFileExtension()
        {
            return "php";
        }
    }
}