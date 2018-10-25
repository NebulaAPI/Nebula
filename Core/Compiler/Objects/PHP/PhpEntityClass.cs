using System.Linq;
using Nebula.Compiler.Abstracts;
using Nebula.Parser;

namespace Core.Compiler.Objects.PHP
{
    public class PhpEntityClass : PhpClass<EntityNode>
    {
        public override void Init()
        {
            Properties.AddRange(RootNode.Fields.Select(f => new AbstractProperty<EntityNode>(f, RootNode)));
        }
    }
}