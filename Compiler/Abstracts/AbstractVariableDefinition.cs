using Nebula.Compiler.Objects;
using Nebula.Parser;

namespace Nebula.Compiler.Abstracts
{
    /// <summary>
    /// The object represents a variable definition for a given language
    /// </summary>
    public class AbstractVariableDefinition : RootObject
    {
        public ArgumentNode Node { get; set; }

        public AbstractDataType DataType { get; set; }
        
        public AbstractVariableDefinition(ArgumentNode node)
        {
            Node = node;
            Name = node.Name;
        }
    }
}