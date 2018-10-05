using System.Collections.Generic;
using Nebula.Compiler.Objects;
using Nebula.Models;

namespace Core.Plugin
{
    public interface ICompilerPlugin
    {
        List<RootObject> GetTopOfClassExtra(ApiConfig config);

        GenericConstructor GetConstructor(string className, ApiConfig config);

        List<GenericProperty> GetProperties();
    }
}