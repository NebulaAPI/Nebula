using System.Collections.Generic;
using System.Linq;
using Nebula.Compiler.Objects;
using Nebula.Parser;

namespace Nebula.Compiler.Abstracts
{
    /// <summary>
    /// This object represents a function for a given language
    /// </summary>
    public class AbstractFunction : RootObject
    {
        public FunctionNode Node { get; set; }

        public AbstractDataType ReturnType { get; set; }

        public List<AbstractVariableDefinition> Arguments { get; set; }

        public AbstractFunction(FunctionNode node)
        {
            Name = node.Name;
            Node = node;
            ReturnType = new AbstractDataType(node.ReturnType);
            Arguments = node.Args.Select(a => new AbstractVariableDefinition(a)).ToList();
        }
    }
}