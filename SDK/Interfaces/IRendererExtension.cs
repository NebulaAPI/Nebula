using System.Collections.Generic;

namespace Nebula.SDK.Plugin
{
    public interface IRendererExtension
    {
        List<string> RenderClientImports();

        List<string> RenderAbstractFunction(string url, string prefix, string returnType, string httpMethod, List<string> args);
    }
}