using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Plugin;
using Nebula.SDK.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Nebula.SDK.Compiler.Objects.Csharp
{
    public class CsharpCompiler : AbstractCompiler
    {
        public CsharpCompiler()
        {
            
        }

        public override List<OutputFile> Compile()
        {
            var outputFiles = new List<OutputFile>();
            
            var entityNamespace = new AbstractNamespace { 
                Name = Project.Name + "." + TemplateMeta.EntityLocation,
                Imports = new List<string> {
                    "System",
                    "System.Collections.Generic"
                }
            };
            var clientNamespace = new AbstractNamespace {
                Name = Project.Name + "." + TemplateMeta.ClientLocation,
                Imports = new List<string> {
                    "System",
                    "System.Collections.Generic",
                    $"{Project.Name}.{TemplateMeta.EntityLocation}"
                }
            };
            
            var entityClasses = GetClassesByType<EntityNode, CsharpEntityClass>(entityNamespace, this);
            var clientClasses = GetClassesByType<ApiNode, CsharpClientClass>(clientNamespace, this);

            outputFiles.AddRange(entityClasses.Select(c => new CsharpOutputFile(c, TemplateMeta)));
            outputFiles.AddRange(clientClasses.Select(c => new CsharpOutputFile(c, TemplateMeta)));
            return outputFiles;
        }
    }
}