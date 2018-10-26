using Nebula.Core.Compiler.Objects;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Abstracts
{
    /// <summary>
    /// This object represents a data type for a given language
    /// </summary>
    public class AbstractDataType : RootObject
    {
        public DataTypeNode Node { get; set; }

        public AbstractDataType(DataTypeNode node)
        {
            Node = node;
            Name = node.Name;
        }
    }
}