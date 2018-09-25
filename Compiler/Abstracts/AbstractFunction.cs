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
        
        public AbstractFunction(FunctionNode node)
        {
            Name = node.Name;
            Node = node;
        }
    }
}