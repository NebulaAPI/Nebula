using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Plugin;
using Nebula.SDK.Renderers;

namespace Nebula.Core.Factories
{
    public class RendererFactory
    {
        public static AbstractRenderer Get(string language, AbstractCompiler compiler, IRendererExtension rendererExtension)
        {
            switch(language.ToLower())
            {
                case "c#": return null;//new CSharpRenderer(compiler, renderPlugin);
                case "php": return null;//new PhpRenderer(compiler, renderPlugin);
                default: return null;
            }    
        }
    }
}