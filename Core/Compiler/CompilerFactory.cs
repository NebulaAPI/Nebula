using Core.Plugin;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Objects.Csharp;
using Nebula.Models;
using Nebula.Parser;

namespace Nebula.Compiler
{
    public static class CompilerFactory
    {
        public static AbstractCompiler Get(string language, Project project, ProjectNode node, TemplateMeta templateData, ICompilerPlugin compilerPlugin)
        {
            switch (language.ToLower())
            {
                case "c#": return new CsharpCompiler(project, node, templateData, compilerPlugin);
                default:
                    throw new System.Exception("Unsupported language: " + language);
            }
        }
    }
}