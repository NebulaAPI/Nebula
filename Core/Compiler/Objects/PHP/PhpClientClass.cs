using System.Linq;
using Nebula.Core.Compiler.Abstracts;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Objects.PHP
{
    public class PhpClientClass : PhpClass<ApiNode>
    {
        public override void Init()
        {
            Config = Compiler.ApiConfig[RootNode];

            Functions.AddRange(RootNode.SearchByType<FunctionNode>().Select(f => new AbstractFunction(f)));

            Properties.AddRange(Compiler.CompilerPlugin.GetProperties());

            Constructor = Compiler.CompilerPlugin.GetConstructor(RootNode.Name, Config);

            TopOfClassExtra.AddRange(Compiler.CompilerPlugin.GetTopOfClassExtra(Config));
        }
    }
}