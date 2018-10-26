using System.Collections.Generic;

namespace Nebula.Core.Compiler.Objects
{
    public class GenericFunction : RootObject
    {
        public string ReturnType { get; set; }

        public List<GenericVariableDefinition> Arguments { get; set; }

        public List<string> Body { get; set; }

        public GenericFunction()
        {
            Arguments = new List<GenericVariableDefinition>();
            Body = new List<string>();
        }
    }
}