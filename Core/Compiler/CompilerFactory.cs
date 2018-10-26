using Nebula.Core.Compiler.Abstracts;
using Nebula.Core.Compiler.Objects.Csharp;
using Nebula.Core.Compiler.Objects.PHP;
using Nebula.Core.Plugin;
using Nebula.SDK.Objects;

namespace Nebula.Core.Compiler
{
    public static class CompilerFactory
    {
        public static AbstractCompiler Get(
            string language,
            Project project,
            ProjectNode node,
            TemplateMeta templateData,
            ICompilerPlugin compilerPlugin
        ) {
            switch (language.ToLower())
            {
                case "c#": return new CsharpCompiler(project, node, templateData, compilerPlugin);
                case "php": return new PhpCompiler(project, node, templateData, compilerPlugin);
                default:
                    throw new System.Exception("Unsupported language: " + language);
            }
        }
    }
}