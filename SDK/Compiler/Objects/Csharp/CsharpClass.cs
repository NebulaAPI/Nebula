using System.Collections.Generic;
using System.Linq;
using System;
using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Objects;

namespace Nebula.SDK.Compiler.Objects.Csharp
{
    public abstract class CsharpClass<T> : AbstractClass<T> where T : MainObjectNode
    {
        public CsharpClass(AbstractNamespace ns, T root, CsharpCompiler compiler) 
            : base(ns, root, compiler)
        {
            
        }

        public CsharpClass()
            : base()
        {

        }

        
    }
}