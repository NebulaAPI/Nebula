using Nebula.Compiler.Abstracts;

namespace Nebula.Renderers
{
    public class RendererFactory
    {
        public static AbstractRenderer Get(string language, AbstractCompiler compiler)
        {
            switch(language.ToLower())
            {
                case "c#": return new CSharpRenderer(compiler);
                default: return null;
            }    
        }
    }
}