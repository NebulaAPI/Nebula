using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System;
using Nebula.SDK.Compiler.Abstracts;
using Nebula.SDK.Plugin;
using Nebula.SDK.Objects;
using Nebula.SDK.Compiler.Objects;
using Nebula.SDK.Util;

namespace Nebula.SDK.Renderers
{
    public class CSharpRenderer : AbstractRenderer
    {
        public CSharpRenderer(AbstractCompiler compiler, IRendererExtension rendererExtension) : base(compiler, rendererExtension)
        {
        }

        protected override string ConvertTypeName(string inputType)
        {
            switch (inputType)
            {
                case "integer": return "int";
                case "boolean": return "bool";
                case "datetime": return "DateTime";
                default: return inputType;
            }
        }

        protected override void RenderAbstractConstructor(AbstractConstructor ac)
        {
            
        }

        protected override string RenderAbstractDataType(AbstractDataType abstractData)
        {
            if (abstractData.Node.Generic && abstractData.Node.Name == "array")
            {
                return $"List<{abstractData.Node.GenericType}>";
            }

            return ConvertTypeName(abstractData.Node.Name);
        }

        private string GetHttpMethod(TokenType functionType)
        {
            switch (functionType)
            {
                case TokenType.GetFunction: return "GET";
                case TokenType.PostFunction: return "POST";
                case TokenType.PutFunction: return "PUT";
                case TokenType.DeleteFunction: return "DELETE";
                case TokenType.PatchFunction: return "PATCH";
                default: throw new System.Exception("Unknown function method type");
            }
        }

        protected override void RenderAbstractFunction(AbstractFunction function)
        {
            var visibility = function.AccessModifier.ToString().ToLower();
            var rt = RenderAbstractDataType(function.ReturnType);
            var args = string.Join(", ", function.Arguments.Select(a => RenderAbstractVariableDefinition(a)));
            var method = GetHttpMethod(function.Node.Method);
            var fname = function.Name.ToProperCase().ToPascalCase();
            var prefix = _activeConfig.Prefix;
            var url = function.Node.Url;

            WriteIndented($"{visibility} {rt} {fname}({args})");
            WriteIndented("{");
            _indentLevel++;
            WriteIndented(
                _rendererExtension.RenderAbstractFunction(
                    url,
                    prefix,
                    rt,
                    method,
                    function.Node.Args.Select(a => a.Name).ToList()
                )
            );
            _indentLevel--;
            WriteIndented("}");
        }

        protected override void RenderAbstractNamespace(AbstractNamespace ns)
        {
            _currentOutput.AddRange(ns.Imports.Select(i => $"using {i};"));
            _currentOutput.AddRange(_rendererExtension.RenderClientImports().Select(i => $"using {i};"));
            _currentOutput.Add($"namespace {ns.Name}");
            _currentOutput.Add("{");
        }

        protected override void RenderAbstractProperty(AbstractProperty<EntityNode> prop)
        {
            var visibility = prop.AccessModifier.ToString().ToLower();
            var rt = RenderAbstractDataType(prop.DataType);
            var fieldName = prop.Name.ToProperCase().ToPascalCase();

            WriteIndented($"{visibility} {rt} {fieldName} {{ get; set; }}");
        }

        protected override string RenderAbstractVariableDefinition(AbstractVariableDefinition variable)
        {
            return $"{RenderAbstractDataType(variable.DataType)} {variable.Name}";
        }

        protected override void RenderApiClass(AbstractClass<ApiNode> ac)
        {
            _activeConfig = ac.Config;
            RenderNode(ac.Namespace);
            _indentLevel++;
            WriteIndented($"public class {ac.Name}Client");
            WriteIndented("{");
            _indentLevel++;
            RenderNodes<RootObject>(ac.TopOfClassExtra);
            RenderNodes<BaseProperty>(ac.Properties);
            RenderNode(ac.Constructor);
            RenderNodes<AbstractFunction>(ac.Functions);
            _indentLevel--;
            WriteIndented("}");
            _indentLevel--;
            WriteIndented("}");
        }

        protected override void RenderEntityClass(AbstractClass<EntityNode> ac)
        {
            RenderNode(ac.Namespace);
            _indentLevel++;
            WriteIndented($"public class {ac.Name}");
            WriteIndented("{");
            _indentLevel++;
            RenderNodes<BaseProperty>(ac.Properties);
            _indentLevel--;
            WriteIndented("}");
            _indentLevel--;
            WriteIndented("}");
        }

        protected override void RenderGenericClass(GenericClass genericClass)
        {
            var inherits = "";
            if (genericClass.Inheritence.Count > 0)
            {
                inherits = ": " + string.Join(", ", genericClass.Inheritence.Select(i => i.Name));
            }
            WriteIndented($"{genericClass.AccessModifier.ToString().ToLower()} class {genericClass.Name} {inherits}");
            WriteIndented("{");
            _indentLevel++;
            RenderGenericProperties(genericClass.Properties);
            RenderGenericConstructor(genericClass.Constructor);
            RenderGenericFunctions(genericClass.Functions);
            _indentLevel--;
            WriteIndented("}");
        }

        protected override void RenderGenericConstructor(GenericConstructor genericConstructor)
        {
            if (genericConstructor == null)
            {
                return;
            }
            var args = string.Join(", ", genericConstructor.Arguments.Select(a => RenderGenericVariableDefinition(a)));
            WriteIndented($"{genericConstructor.AccessModifier.ToString().ToLower()} {genericConstructor.Name}({args})");
            WriteIndented("{");
            _indentLevel++;
            WriteIndented(genericConstructor.Body);
            _indentLevel--;
            WriteIndented("}");
        }

        protected override void RenderGenericFunction(GenericFunction genericFunction)
        {
            var visibility = genericFunction.AccessModifier.ToString().ToLower();
            var rt = genericFunction.ReturnType;
            var name = genericFunction.Name;
            var args = string.Join(", ", genericFunction.Arguments.Select(a => RenderGenericVariableDefinition(a)));
            WriteIndented($"{visibility} {rt} {name}({args})");
            WriteIndented("{");
            _indentLevel++;
            WriteIndented(genericFunction.Body);
            _indentLevel--;
            WriteIndented("}");
        }

        protected override void RenderGenericProperty(GenericProperty prop)
        {
            WriteIndented($"{prop.AccessModifier.ToString().ToLower()} {prop.DataTypeString} {prop.Name} {{ get; set; }}");
        }

        protected override string RenderGenericVariableDefinition(GenericVariableDefinition variableDefinition)
        {
            return $"{variableDefinition.DataTypeName} {variableDefinition.Name}";
        }

        protected override void RenderGenericTryCatch(GenericTryCatch tryCatch)
        {
            WriteIndented("try");
            WriteIndented("{");
            _indentLevel++;
            WriteIndented(tryCatch.Body);
            _indentLevel--;
            WriteIndented("}");
            foreach (var catchBlock in tryCatch.CatchExceptions.Keys)
            {
                WriteIndented($"catch ({catchBlock} {tryCatch.CatchExceptions[catchBlock]})");
                WriteIndented("{");
                WriteIndented("}");
            }
        }

        protected override List<string> RenderDocBlock(string description, Dictionary<string, string> paramValues)
        {
            throw new NotImplementedException();
        }
    }
}