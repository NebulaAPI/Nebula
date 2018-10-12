using System.Linq;
using Nebula.Compiler.Abstracts;
using Nebula.Parser;

namespace Nebula.Compiler.Objects.Csharp
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