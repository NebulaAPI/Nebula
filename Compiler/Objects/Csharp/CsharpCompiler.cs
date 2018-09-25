using Nebula.Compiler.Abstracts;
using Nebula.Models;
using Nebula.Parser;
using System.Linq;

namespace Nebula.Compiler.Objects.Csharp
{
    public class CsharpCompiler : AbstractCompiler
    {
        /// <summary>
        /// The output of the compiler is a collection of OutputFile objects that contain the
        /// structure to be rendered
        /// </summary>
        /// <param name="rootNode"></param>
        public CsharpCompiler(Project project, ProjectNode rootNode, TemplateMeta templateData)
        {
            var entityNamespace = new CsharpNamespace { Name = templateData.EntityLocation };
            var clientNamespace = new CsharpNamespace { Name = templateData.ClientLocation };
            entityNamespace.Classes.AddRange(rootNode.SearchByType<EntityNode>().Select(e => new CsharpClass(e)));
            clientNamespace.Classes.AddRange(rootNode.SearchByType<ApiNode>().Select(a => new CsharpClass(a)));
        }
    }
}