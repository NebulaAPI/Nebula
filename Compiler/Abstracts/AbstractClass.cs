using Nebula.Compiler.Objects;
using Nebula.Parser;

namespace Nebula.Compiler.Abstracts
{
    /// <summary>
    /// This object represents a 'class' construct within a given language.
    /// </summary>
    public class AbstractClass : RootObject
    {
        protected AstNode RootNode { get; set; }
        
        public AbstractClass(ApiNode root)
        {
            RootNode = root;
            Name = root.Name;
        }

        public AbstractClass(EntityNode root)
        {
            RootNode = root;
            Name = root.Name;
        }
    }
}