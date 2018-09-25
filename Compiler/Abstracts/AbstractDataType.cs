using Nebula.Compiler.Objects;
using Nebula.Parser;

namespace Nebula.Compiler.Abstracts
{
    /// <summary>
    /// This object represents a data type for a given language
    /// </summary>
    public class AbstractDataType : RootObject
    {
        public DataTypeNode Node { get; set; }

        protected AbstractDataType(DataTypeNode node)
        {
            Node = node;
            Name = node.Name;
        }
    }
}