using System.Collections.Generic;
using System.Linq;
using Nebula.Core.Compiler.Abstracts;
using Nebula.Core.Plugin;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler.Objects.PHP
{
    public class PhpCompiler : AbstractCompiler
    {
        public PhpCompiler(Project project, ProjectNode projectNode, TemplateMeta meta, ICompilerPlugin compilerPlugin)
            : base(project, projectNode, meta, compilerPlugin)
        {
            var projectName = project.GetProperName();
            
            var entityNamespace = new AbstractNamespace { 
                Name = projectName + @"\" + meta.EntityLocation
            };

            var clientNamespace = new AbstractNamespace {
                Name = projectName + @"\" + meta.ClientLocation,
                Imports = new List<string> {
                    
                    @"JsonMapper",
                    $"{projectName}\\{meta.EntityLocation}"
                }
            };

            var entityClasses = GetClassesByType<EntityNode, PhpEntityClass>(entityNamespace, this);
            var clientClasses = GetClassesByType<ApiNode, PhpClientClass>(clientNamespace, this);

            OutputFiles.AddRange(entityClasses.Select(c => new PhpOutputFile(c, meta)));
            OutputFiles.AddRange(clientClasses.Select(c => new PhpOutputFile(c, meta)));
        }
    }
}