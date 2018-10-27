using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Compiler.Objects.Csharp;
using Nebula.SDK.Compiler.Objects.PHP;
using Nebula.SDK.Plugin;
using Nebula.SDK.Objects;

namespace Nebula.SDK.Compiler
{
    public static class CompilerFactory
    {
        public static AbstractCompiler Get(string language)
        {
            switch (language.ToLower())
            {
                case "c#": return new CsharpCompiler();
                case "php": return new PhpCompiler();
                default:
                    throw new System.Exception("Unsupported language: " + language);
            }
        }
    }
}