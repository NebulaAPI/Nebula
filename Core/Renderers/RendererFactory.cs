using Core.Plugin;
using Nebula.Compiler.Abstracts;

namespace Nebula.Renderers
{
    public class RendererFactory
    {
        public static AbstractRenderer Get(string language, AbstractCompiler compiler, IRenderPlugin renderPlugin)
        {
            switch(language.ToLower())
            {
                case "c#": return new CSharpRenderer(compiler, renderPlugin);
                default: return null;
            }    
        }
    }
}