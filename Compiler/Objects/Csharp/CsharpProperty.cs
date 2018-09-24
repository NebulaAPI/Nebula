using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Interfaces;
using Nebula.Parser;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpProperty : AbstractProperty, IRenderable
    {
        public CsharpProperty(ArgumentNode root)
        {
            Name = root.Name;
        }

        public string Render()
        {
            throw new System.NotImplementedException();
        }
    }
}