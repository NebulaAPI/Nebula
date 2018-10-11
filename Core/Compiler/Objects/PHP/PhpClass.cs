using Nebula.Compiler.Abstracts;
using Nebula.Parser;

namespace Core.Compiler.Objects.PHP
{
    public abstract class PhpClass<T> : AbstractClass<T> where T : MainObjectNode
    {
        public PhpClass()
        {
        }

        public PhpClass(AbstractNamespace ns, T root, AbstractCompiler compiler) : base(ns, root, compiler)
        {
        }
    }
}