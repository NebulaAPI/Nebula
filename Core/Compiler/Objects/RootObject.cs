using System.Linq;

namespace Nebula.Core.Compiler.Objects
{
    public enum Visibility
    {
        Public,
        Protected,
        Private
    }
    
    /// <summary>
    /// This object represents the top of the hierarchy of compiler components.
    /// </summary>
    public class RootObject
    {
        public string Name { get; set; }

        public Visibility AccessModifier { get; set; }

        public RootObject()
        {
            AccessModifier = Visibility.Public;
        }

        protected string T(int tabLevel)
        {
            return new string('\t', tabLevel);
        }
    }
}