using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Plugin;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Nebula.Models;

namespace Core.Services
{
    /// <summary>
    /// This service is responsible for creating a plugin class instance from the 
    /// specified source file
    /// </summary>
    public class PluginService
    {
        private Assembly CompiledAssembly { get; set; }

        private string[] SourceFiles { get; set; }

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

        public PluginService(params string[] sourceFiles)
        {
            CompiledAssembly = null;
            SourceFiles = sourceFiles;
        }

        private void GenerateInMemoryAssembly()
        {
            var sourceLanguage = new CSharpLanguage();
            var syntaxTrees = SourceFiles.Select(s => sourceLanguage.ParseText(File.ReadAllText(s), SourceCodeKind.Regular));

            Compilation compilation = sourceLanguage
                .CreateLibraryCompilation(assemblyName: "InMemoryAssembly", enableOptimisations: false)
                .AddSyntaxTrees(syntaxTrees.ToArray());

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            if (emitResult.Success) {
                stream.Seek(0, SeekOrigin.Begin);
                CompiledAssembly = Assembly.Load(stream.ToArray());
            } else {
                var errorMsg = string.Join("\n", emitResult.Diagnostics.Select(d => $"{d.Id} {d.GetMessage()}"));
                throw new Exception(errorMsg);
            }
        }
        
        public T GetPlugin<T>()
        {
            if (CompiledAssembly == null)
            {
                GenerateInMemoryAssembly();
            }
            var type = CompiledAssembly.GetTypes().FirstOrDefault(t => t.GetInterfaces().Any(i => i.Name.Contains(typeof(T).Name)));
            if (type == null)
            {
                throw new Exception($"Could not find class that implements interface {typeof(T).Name}");
            }
            return (T)Activator.CreateInstance(type);
        }
    }
}