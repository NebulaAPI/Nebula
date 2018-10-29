using Nebula.SDK.Compiler.Abstracts;
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
                case "c#": return null;//new CsharpCompiler();
                case "php": return null;//new PhpCompiler();
                default:
                    throw new System.Exception("Unsupported language: " + language);
            }
        }
    }
}