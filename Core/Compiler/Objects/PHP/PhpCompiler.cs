using System.Collections.Generic;
using System.Linq;
using Core.Plugin;
using Nebula.Compiler.Abstracts;
using Nebula.Models;
using Nebula.Parser;
using Nebula.Util;

namespace Core.Compiler.Objects.PHP
{
    public class PhpCompiler : AbstractCompiler
    {
        public PhpCompiler(Project project, ProjectNode projectNode, TemplateMeta meta, ICompilerPlugin compilerPlugin)
            : base(project, projectNode, meta, compilerPlugin)
        {
            var projectName = project.Name.Replace("-", " ").Replace("_", " ").ToProperCase().ToPascalCase();
            
            var entityNamespace = new AbstractNamespace { 
                Name = projectName + @"\" + meta.EntityLocation
            };

            var clientNamespace = new AbstractNamespace {
                Name = projectName + @"\" + meta.ClientLocation,
                Imports = new List<string> {
                    //@"GuzzleHttp\Client",
                    //@"GuzzleHttp\Exception\GuzzleException",
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