using System.Collections.Generic;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Interfaces;
using Nebula.Parser;
using System.Linq;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpFunction : AbstractFunction, IRenderable
    {
        public CsharpDataType ReturnType { get; set; }

        public List<CsharpVariableDefinition> Arguments { get; set; }
        
        public CsharpFunction(FunctionNode node) : base(node)
        {
            ReturnType = new CsharpDataType(node.ReturnType);

            Arguments = node.Args.Select(a => new CsharpVariableDefinition(a)).ToList();
        }

        public string Render()
        {
            throw new System.NotImplementedException();
        }
    }
}