namespace Nebula.Compiler.Objects
{
    public class GenericVariableDefinition : RootObject
    {
        public string DataTypeName { get; set; }
        
        public GenericVariableDefinition(string name, string type)
        {
            Name = name;
            DataTypeName = type;
        }
    }
}