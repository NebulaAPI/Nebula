using System.IO;
using Nebula.Models;
using Nebula.Parser;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpOutputFile : OutputFile
    {
        public CsharpOutputFile(RootObject root, TemplateMeta meta) : base(root)
        {
            switch (root)
            {
                case CsharpEntityClass e:
                    FileName = Path.Join(meta.EntityLocation, $"{e.Name}.{GetFileExtension()}");
                    break;
                case CsharpClientClass c:
                    FileName = Path.Join(meta.ClientLocation, $"{c.Name}Client.{GetFileExtension()}");
                    break;
            }
        }

        protected override string GetFileExtension()
        {
            return "cs";
        }
    }
}