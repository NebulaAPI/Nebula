using System.Linq;
using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Compiler.Objects.Csharp;
using Nebula.SDK.Objects;

namespace Nebula.SDK.Compiler.Objects.Csharp
{
    public class CsharpEntityClass : CsharpClass<EntityNode>
    {
        public CsharpEntityClass()
        {
            
        }

        public override void Init()
        {
            Properties.AddRange(RootNode.Fields.Select(f => new AbstractProperty<EntityNode>(f, RootNode)));
        }
    }
}