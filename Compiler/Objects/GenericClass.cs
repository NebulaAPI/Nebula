using System.Collections.Generic;
using Nebula.Compiler.Abstracts;

namespace Nebula.Compiler.Objects
{
    public class GenericClass : RootObject
    {
        public List<GenericClass> Inheritence { get; set; }

        public List<GenericProperty> Properties { get; set; }

        public AbstractConstructor Constructor { get; set; }

        public List<AbstractFunction> Functions { get; set; }

        public GenericClass()
        {
            Inheritence = new List<GenericClass>();
            Properties = new List<GenericProperty>();
            Functions = new List<AbstractFunction>();
        }
    }
}