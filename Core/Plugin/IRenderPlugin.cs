using System.Collections.Generic;
using Nebula.Compiler.Abstracts;
using Nebula.Parser;

namespace Core.Plugin
{
    public interface IRenderPlugin
    {
        List<string> RenderClientImports();

        List<string> RenderAbstractFunction(string url, string prefix, string returnType, string httpMethod, List<string> args);
    }
}