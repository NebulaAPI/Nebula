using System.Collections.Generic;
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

        public string ReturnTypeString { get; set; }

        public List<AbstractVariableDefinition> Arguments { get; set; }

        public List<string> Body { get; set; }
        
        public AbstractFunction(FunctionNode node) : this()
        {
            Name = node.Name;
            Node = node;
        }

        public AbstractFunction()
        {
            Arguments = new List<AbstractVariableDefinition>();
            Body = new List<string>();
        }
    }
}