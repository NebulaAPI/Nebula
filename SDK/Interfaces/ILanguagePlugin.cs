using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Renderers;

namespace Nebula.SDK.Interfaces
{
    public interface ILanguagePlugin
    {
        string GetLanguageName();
        AbstractCompiler GetCompiler();
        AbstractRenderer GetRenderer();
    }
}