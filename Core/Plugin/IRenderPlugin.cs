using System.Collections.Generic;

namespace Core.Plugin
{
    public interface IRenderPlugin
    {
        List<string> RenderClientImports();
    }
}