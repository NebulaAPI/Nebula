using System.Collections.Generic;
using System.Linq;
using System;
using Nebula.Core.Compiler.Abstracts;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Objects.Csharp
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