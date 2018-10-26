using System.Collections.Generic;

namespace Nebula.Core.Compiler.Objects
{
    public class GenericConstructor : RootObject
    {
        public List<GenericVariableDefinition> Arguments { get; set; }

        public List<string> Body { get; set; }

        public GenericConstructor()
        {
            Arguments = new List<GenericVariableDefinition>();
            Body = new List<string>();
        }
    }
}