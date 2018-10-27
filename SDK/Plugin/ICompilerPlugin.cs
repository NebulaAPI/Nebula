using System.Collections.Generic;
using Nebula.SDK.Compiler.Objects;
using Nebula.SDK.Objects;

namespace Nebula.SDK.Plugin
{
    public interface ICompilerPlugin
    {
        List<RootObject> GetTopOfClassExtra(ApiConfig config);

        GenericConstructor GetConstructor(string className, ApiConfig config);

        List<GenericProperty> GetProperties();
    }
}