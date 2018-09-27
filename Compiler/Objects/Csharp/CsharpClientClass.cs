using System.Linq;
using Nebula.Compiler.Abstracts;
using Nebula.Parser;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpClientClass : CsharpClass<ApiNode>
    {
        public CsharpClientClass() 
        {
            
        }

        public override void Init()
        {
            Config = Compiler.ApiConfig[RootNode];

            Functions.AddRange(RootNode.SearchByType<FunctionNode>().Select(f => new AbstractFunction(f)));

            Properties.Add(new GenericProperty("Client", "RestClient"));

            Constructor = BuildConstructor();

            var authenticator = BuildAuthenticator();
            if (authenticator != null)
            {
                TopOfClassExtra.Add(authenticator);
            }
        }
    }
}