using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Plugin;

namespace Nebula.SDK.Renderers
{
    public class RendererFactory
    {
        public static AbstractRenderer Get(string language, AbstractCompiler compiler, IRenderPlugin renderPlugin)
        {
            switch(language.ToLower())
            {
                case "c#": return new CSharpRenderer(compiler, renderPlugin);
                case "php": return new PhpRenderer(compiler, renderPlugin);
                default: return null;
            }    
        }
    }
}