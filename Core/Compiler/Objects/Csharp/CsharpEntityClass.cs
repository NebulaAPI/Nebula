using System.Linq;
using Nebula.Core.Compiler.Abstracts;
using Nebula.Core.Compiler.Objects.Csharp;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Objects.Csharp
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