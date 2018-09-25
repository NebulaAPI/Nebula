using Nebula.Compiler.Objects;
using Nebula.Parser;

namespace Nebula.Compiler.Abstracts
{
    /// <summary>
    /// This object represents a property with a getter and setter for a given language.
    /// </summary>
    public class AbstractProperty : RootObject
    {
        public AbstractDataType DataType { get; set; }

        public ArgumentNode Node { get; set; }

        protected AbstractProperty(ArgumentNode node)
        {
            Name = node.Name;
            Node = node;
        }
    }
}