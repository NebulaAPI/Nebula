using System;
using System.Collections.Generic;
using System.Linq;
using Nebula.Core.Compiler.Objects;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Abstracts
{
    /// <summary>
    /// This object represents a 'class' construct within a given language.
    /// </summary>
    public abstract class AbstractClass<T> : RootObject where T : MainObjectNode
    {
        public T RootNode { get; set; }

        public AbstractNamespace Namespace { get; set; }

        public GenericConstructor Constructor { get; set; }

        public AbstractCompiler Compiler { get; set; }

        public List<RootObject> TopOfClassExtra { get; set; }

        public List<BaseProperty> Properties { get; set; }

        public List<AbstractFunction> Functions { get; set; }

        public ApiConfig Config { get; set; }

        public AbstractClass(AbstractNamespace ns, T root, AbstractCompiler compiler)
            : this()
        {
            RootNode = root;
            Name = root.Name;
            Namespace = ns;
            Compiler = compiler;
        }

        protected AbstractClass()
        {
            TopOfClassExtra = new List<RootObject>();
            Properties = new List<BaseProperty>();
            Functions = new List<AbstractFunction>();
        }

        public U Build<U>(AbstractNamespace ns, T root, AbstractCompiler compiler)
            where U : AbstractClass<T>, new()
        {
            var cls = new U() {
                RootNode = root,
                Name = root.Name,
                Namespace = ns,
                Compiler = compiler
            };

            cls.Init();
            return cls;
        }

        public abstract void Init();
        
    }
}