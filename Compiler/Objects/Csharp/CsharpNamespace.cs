using System.Collections.Generic;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Interfaces;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpNamespace : AbstractNamespace, IRenderable
    {
        public List<CsharpClass> Classes { get; set; }

        public CsharpNamespace()
        {
            Classes = new List<CsharpClass>();
        }
        
        public string Render()
        {
            throw new System.NotImplementedException();
        }
    }
}