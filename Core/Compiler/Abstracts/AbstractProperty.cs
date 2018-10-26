using Nebula.Core.Compiler.Objects;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Abstracts
{
    /// <summary>
    /// This object represents a property with a getter and setter for a given language.
    /// </summary>
    public class AbstractProperty<T> : BaseProperty
    {
        public AbstractDataType DataType { get; set; }

        public ArgumentNode Node { get; set; }

        public T Parent { get; set; }

        public AbstractProperty(ArgumentNode node, T parent)
        {
            Name = node.Name;
            Node = node;
            Parent = parent;
            DataType = new AbstractDataType(node.ArgType);
        }
    }
}