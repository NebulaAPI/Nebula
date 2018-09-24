namespace Nebula.Compiler.Objects
{
    public class OutputFile
    {
        public RootObject Root { get; set; }
        
        public OutputFile(RootObject root)
        {
            Root = root;
        }
    }
}