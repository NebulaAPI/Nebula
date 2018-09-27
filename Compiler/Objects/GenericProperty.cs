namespace Nebula.Compiler.Objects
{
    public class GenericProperty : BaseProperty
    {
        public string DataTypeString { get; set; }

        public GenericProperty(string name, string type)
        {
            Name = name;
            DataTypeString = type;
        }

        public GenericProperty()
        {
            
        }    
    }
}