using System.Collections.Generic;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Interfaces;
using Nebula.Parser;
using System.Linq;
using System;
using Nebula.Models;

namespace Nebula.Compiler.Objects.Csharp
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