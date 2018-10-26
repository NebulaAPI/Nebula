using System.Linq;
using Nebula.Core.Compiler.Abstracts;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Objects.PHP
{
    public class PhpEntityClass : PhpClass<EntityNode>
    {
        public override void Init()
        {
            Properties.AddRange(RootNode.Fields.Select(f => new AbstractProperty<EntityNode>(f, RootNode)));
        }
    }
}