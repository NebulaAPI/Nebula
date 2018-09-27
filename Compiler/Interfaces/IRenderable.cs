using System.Collections.Generic;

namespace Nebula.Compiler.Interfaces
{
    public interface IRenderable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<string> Render(int initialIndentLevel);
    }
}