using System.Collections.Generic;

namespace Nebula.Core.Plugin
{
    public interface IRenderPlugin
    {
        List<string> RenderClientImports();

        List<string> RenderAbstractFunction(string url, string prefix, string returnType, string httpMethod, List<string> args);
    }
}