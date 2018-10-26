using System.Collections.Generic;
using Nebula.Core.Compiler.Objects;
using Nebula.SDK.Objects;

namespace Nebula.Core.Plugin
{
    public interface ICompilerPlugin
    {
        List<RootObject> GetTopOfClassExtra(ApiConfig config);

        GenericConstructor GetConstructor(string className, ApiConfig config);

        List<GenericProperty> GetProperties();
    }
}