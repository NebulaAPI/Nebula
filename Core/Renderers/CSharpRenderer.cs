using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Nebula.Models;
using Nebula.Parser;
using Core.Services;
using Nebula.Util;
using System.Linq;
using System;
using Nebula.Compiler.Objects.Csharp;
using Nebula.Compiler.Objects;
using Nebula.Compiler.Abstracts;
using Core.Plugin;

namespace Nebula.Renderers
{
    public class CSharpRenderer : AbstractRenderer
    {
        public CSharpRenderer(AbstractCompiler compiler, IRenderPlugin renderPlugin) : base(compiler, renderPlugin)
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
                case TokenType.GetFunction: return "Method.GET";
                case TokenType.PostFunction: return "Method.POST";
                case TokenType.PutFunction: return "Method.PUT";
                case TokenType.DeleteFunction: return "Method.DELETE";
                case TokenType.PatchFunction: return "Method.PATCH";
                default: throw new System.Exception("Unknown function method type");
            }
        }

        private List<string> RenderUrlSegment(string url, List<ArgumentNode> args)
        {
            var output = new List<string>();
            // look in the URL for {variable} strings and then try and find a matching function argument
            // if we find it, generate the appropriate request.AddUrlSegment call
            // for any argument that is not part of the URL, send that as a parameter
            var regex = new Regex(@"({[a-z]+})", RegexOptions.IgnoreCase);
            var matches = regex.Matches(url);
            var usedArgs = new List<ArgumentNode>();
            foreach (Match m in matches)
            {
                var parameterName = m.Value.Replace("{", "").Replace("}", "");
                var matchingArg = args.Where(a => a.Name == parameterName).FirstOrDefault() 
                    ?? throw new Exception("No matching argument for URL parameter: " + parameterName);
                
                usedArgs.Add(matchingArg);
                output.Add($"request.AddUrlSegment(\"{parameterName}\", {matchingArg.Name});");
            }

            var unusedArgs = args.Where(a => !usedArgs.Contains(a));
            output.AddRange(unusedArgs.Select(arg => $"request.AddParameter(\"{arg.Name}\", {arg.Name});"));

            return output;
        }

        protected override void RenderAbstractFunction(AbstractFunction function)
        {
            var visibility = function.AccessModifier.ToString().ToLower();
            var rt = RenderAbstractDataType(function.ReturnType);
            var args = string.Join(", ", function.Arguments.Select(a => RenderAbstractVariableDefinition(a)));
            var method = GetHttpMethod(function.Node.Method);
            var fname = function.Name.ToProperCase().ToPascalCase();
            var prefix = ActiveConfig.Prefix;
            var url = function.Node.Url;

            WriteIndented($"{visibility} {rt} {fname}({args})");
            WriteIndented("{");
            IndentLevel++;
            WriteIndented($"var request = new RestRequest(\"{prefix}{url}\");");
            WriteIndented(RenderUrlSegment(url, function.Node.Args));
            WriteIndented($"var response = Client.Execute<{rt}>(request);");
            WriteIndented("return response.Data;");
            IndentLevel--;
            WriteIndented("}");
        }

        protected override void RenderAbstractNamespace(AbstractNamespace ns)
        {
            CurrentOutput.AddRange(ns.Imports.Select(i => $"using {i};"));
            CurrentOutput.AddRange(RenderPlugin.RenderClientImports().Select(i => $"using {i};"));
            CurrentOutput.Add($"namespace {ns.Name}");
            CurrentOutput.Add("{");
        }

        protected override void RenderAbstractProperty(AbstractProperty prop)
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
            ActiveConfig = ac.Config;
            RenderNode(ac.Namespace);
            IndentLevel++;
            WriteIndented($"public class {ac.Name}Client");
            WriteIndented("{");
            IndentLevel++;
            RenderNodes<RootObject>(ac.TopOfClassExtra);
            RenderNodes<BaseProperty>(ac.Properties);
            RenderNode(ac.Constructor);
            RenderNodes<AbstractFunction>(ac.Functions);
            IndentLevel--;
            WriteIndented("}");
            IndentLevel--;
            WriteIndented("}");
        }

        protected override void RenderEntityClass(AbstractClass<EntityNode> ac)
        {
            RenderNode(ac.Namespace);
            IndentLevel++;
            WriteIndented($"public class {ac.Name}");
            WriteIndented("{");
            IndentLevel++;
            RenderNodes<BaseProperty>(ac.Properties);
            IndentLevel--;
            WriteIndented("}");
            IndentLevel--;
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
            IndentLevel++;
            RenderGenericProperties(genericClass.Properties);
            RenderGenericConstructor(genericClass.Constructor);
            RenderGenericFunctions(genericClass.Functions);
            IndentLevel--;
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
            IndentLevel++;
            WriteIndented(genericConstructor.Body);
            IndentLevel--;
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
            IndentLevel++;
            WriteIndented(genericFunction.Body);
            IndentLevel--;
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
    }
}