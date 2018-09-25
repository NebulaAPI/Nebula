using Nebula.Compiler.Abstracts;
using Nebula.Parser;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpVariableDefinition : AbstractVariableDefinition
    {
        public CsharpVariableDefinition(ArgumentNode node) : base(node)
        {
            DataType = new CsharpDataType(node.ArgType);    
        }
    }
}