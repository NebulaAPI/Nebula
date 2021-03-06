using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Nebula.SDK.Plugin;
using Nebula.SDK.Util;

namespace Nebula.Core.Services.Client
{
    public interface ICompilationService
    {
        Assembly CompileInMemory(string name, params string[] files);
        Assembly CompileLocal(string name, string outputFile, params string[] files);
    }
    
    /// <summary>
    /// Class to handle generating a local or in-memory assembly
    /// </summary>
    public class CompilationService : ICompilationService
    {
        private IFileUtil _fileUtil;

        public CompilationService(IFileUtil fileUtil)
        {
            _fileUtil = fileUtil;
        }
        
        public Assembly CompileInMemory(string name, params string[] files)
        {
            var compilation = GenerateCompilation(name, files);

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
        
        public Assembly CompileLocal(string name, string outputFile, params string[] files)
        {
            var compilation = GenerateCompilation(name, files);

            var fs = _fileUtil.FileCreate(outputFile);
            var result = compilation.Emit(fs);
            fs.Close();
            if (result.Success) {
                return Assembly.LoadFile(outputFile);
            } else {
                var errorMsg = string.Join("\n", result.Diagnostics.Select(d => $"{d.Id} {d.GetMessage()}"));
                throw new Exception(errorMsg);
            }
        }

        private Compilation GenerateCompilation(string assemblyName, params string[] files)
        {
            var sourceLanguage = new CSharpLanguage();
            var syntaxTrees = files.Select(s => sourceLanguage.ParseText(_fileUtil.FileReadAllText(s), SourceCodeKind.Regular));

            return sourceLanguage
                .CreateLibraryCompilation(assemblyName: assemblyName, enableOptimisations: false)
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
                MetadataReference.CreateFromFile(typeof(IRendererExtension).GetTypeInfo().Assembly.Location),
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