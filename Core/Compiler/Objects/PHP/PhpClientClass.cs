using System.Linq;
using Nebula.Compiler.Abstracts;
using Nebula.Parser;

namespace Core.Compiler.Objects.PHP
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