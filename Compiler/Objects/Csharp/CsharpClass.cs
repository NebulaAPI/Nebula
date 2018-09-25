using System.Collections.Generic;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Interfaces;
using Nebula.Parser;
using System.Linq;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpClass : AbstractClass, IRenderable
    {
        public List<CsharpProperty> Properties { get; set; }

        public List<CsharpFunction> Functions { get; set; }

        public CsharpClass(ApiNode root) : base(root)
        {
            Init();

            Functions.AddRange(root.SearchByType<FunctionNode>().Select(f => new CsharpFunction(f)));
        }

        public CsharpClass(EntityNode root) : base(root)
        {
            Init();

            Properties.AddRange(root.Fields.Select(f => new CsharpProperty(f)));
        }

        private void Init()
        {
            Properties = new List<CsharpProperty>();
            Functions = new List<CsharpFunction>();
        }

        public string Render()
        {
            throw new System.NotImplementedException();
        }
    }
}