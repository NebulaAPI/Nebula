using System.Collections.Generic;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Interfaces;
using Nebula.Parser;
using System.Linq;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpFunction : AbstractFunction
    {
        public AbstractDataType ReturnType { get; set; }

        public CsharpFunction(FunctionNode node) : base(node)
        {
            ReturnType = new AbstractDataType(node.ReturnType);

            Arguments = node.Args.Select(a => new AbstractVariableDefinition(a)).ToList();
        }
    }
}