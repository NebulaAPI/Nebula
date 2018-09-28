using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Objects.Csharp;
using Nebula.Models;
using Nebula.Parser;

namespace Nebula.Compiler
{
    public static class CompilerFactory
    {
        public static AbstractCompiler Get(string language, Project project, ProjectNode node, TemplateMeta templateData)
        {
            switch (language.ToLower())
            {
                case "c#": return new CsharpCompiler(project, node, templateData);
                default:
                    throw new System.Exception("Unsupported language: " + language);
            }
        }
    }
}