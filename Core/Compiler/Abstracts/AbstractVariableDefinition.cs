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

        public string DataTypeString { get; set; }
        
        public AbstractVariableDefinition(ArgumentNode node)
        {
            Node = node;
            Name = node.Name;
            DataType = new AbstractDataType(node.ArgType);
        }

        public AbstractVariableDefinition(string name, string type)
        {
            Name = name;
            DataTypeString = type;
        }
    }
}