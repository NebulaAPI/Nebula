using System.IO;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Objects;
using Nebula.Models;
using Nebula.Parser;

namespace Core.Compiler.Objects.PHP
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