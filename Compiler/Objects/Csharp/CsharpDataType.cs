using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Interfaces;
using Nebula.Parser;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpDataType : AbstractDataType, IRenderable
    {
        public CsharpDataType(DataTypeNode node) : base(node)
        {
        }

        public string Render()
        {
            throw new System.NotImplementedException();
        }
    }
}