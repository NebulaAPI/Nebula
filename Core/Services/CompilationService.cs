using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Nebula.SDK.Plugin;

namespace Nebula.Core.Services
{
    /// <summary>
    /// Class to handle generating a local or in-memory assembly
    /// </summary>
    public class CompilationService
    {
        public Assembly CompileInMemory(params string[] files)
        {
            var compilation = GenerateCompilation(files);

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            if (emitResult.Success) {
                stream.Seek(0, SeekOrigin.Begin);
                return Assembly.Load(stream.ToArray());
            } else {
                var errorMsg = string.Join("\n", emitResult.Diagnostics.Select(d => $"{d.Id} {d.GetMessage()}"));
                throw new Exception(errorMsg);
            }
        }
        
        public Assembly CompileLocal(string outputFile, params string[] files)
        {
            var compilation = GenerateCompilation(files);

            var fs = File.Create(outputFile);
            var result = compilation.Emit(fs);
            fs.Close();
            if (result.Success) {
                return Assembly.LoadFile(outputFile);
            } else {
                var errorMsg = string.Join("\n", result.Diagnostics.Select(d => $"{d.Id} {d.GetMessage()}"));
                throw new Exception(errorMsg);
            }
        }

        private Compilation GenerateCompilation(params string[] files)
        {
            var sourceLanguage = new CSharpLanguage();
            var syntaxTrees = files.Select(s => sourceLanguage.ParseText(File.ReadAllText(s), SourceCodeKind.Regular));

            return sourceLanguage
                .CreateLibraryCompilation(assemblyName: "InMemoryAssembly", enableOptimisations: false)
                .AddSyntaxTrees(syntaxTrees.ToArray());
        }
        
        private interface ILanguageService
        {
            SyntaxTree ParseText(string code, SourceCodeKind kind);

            Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations);
        }

        private class CSharpLanguage : ILanguageService
        {
            private readonly List<PortableExecutableReference> _references = new List<PortableExecutableReference> {
                MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IRenderPlugin).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<string>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(IEnumerable<>).GetTypeInfo().Assembly.Location),
                MetadataReference.CreateFromFile(typeof(List<string>).GetTypeInfo().Assembly.Location),
            };
            
            private static readonly LanguageVersion MaxLanguageVersion = Enum
                .GetValues(typeof(LanguageVersion))
                .Cast<LanguageVersion>()
                .Max();

            public SyntaxTree ParseText(string sourceCode, SourceCodeKind kind) {
                var options = new CSharpParseOptions(kind: kind, languageVersion: MaxLanguageVersion);

                // Return a syntax tree of our source code
                return CSharpSyntaxTree.ParseText(sourceCode, options);
            }

            public Compilation CreateLibraryCompilation(string assemblyName, bool enableOptimisations) {
                var options = new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: enableOptimisations ? OptimizationLevel.Release : OptimizationLevel.Debug,
                    allowUnsafe: true);

                var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
                var neededAssemblies = new[]
                {
                    "System.Runtime",
                    "mscorlib",
                };
                var references = trustedAssembliesPaths
                    .Select(p => MetadataReference.CreateFromFile(p))
                    .ToList();

                references.AddRange(_references);

                return CSharpCompilation.Create(assemblyName, options: options, references: references);
            }
        }
    }
}