using System.Collections.Generic;
using System.Linq;
using Core.Compiler.Objects;
using Core.Plugin;
using Nebula.Compiler.Abstracts;
using Nebula.Compiler.Objects;
using Nebula.Parser;
using Nebula.Renderers;
using Nebula.Util;

namespace Core.Renderers
{
    public class PhpRenderer : AbstractRenderer
    {
        public PhpRenderer(AbstractCompiler compiler, IRenderPlugin renderPlugin) : base(compiler, renderPlugin)
        {
        }

        protected override string ConvertTypeName(string inputType)
        {
            switch (inputType)
            {
                case "integer": return "int";
                case "boolean": return "bool";
                case "datetime": return @"\DateTime";
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
                return $"{abstractData.Node.GenericType}[]";
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
            var prefix = ActiveConfig.Prefix;
            var url = function.Node.Url;

            WriteIndented($"{visibility} function {fname}({args}) : {rt}");
            WriteIndented("{");
            IndentLevel++;
            WriteIndented(
                RenderPlugin.RenderAbstractFunction(
                    url,
                    prefix,
                    rt,
                    method,
                    function.Node.Args.Select(a => a.Name).ToList()
                )
            );
            IndentLevel--;
            WriteIndented("}");
        }

        protected override void RenderAbstractNamespace(AbstractNamespace ns)
        {
            CurrentOutput.AddRange(ns.Imports.Select(i => $"use {i};"));
            CurrentOutput.AddRange(RenderPlugin.RenderClientImports().Select(i => $"use {i};"));
            CurrentOutput.Add($"namespace {ns.Name};");
        }

        protected override void RenderAbstractProperty(AbstractProperty prop)
        {
            var visibility = prop.AccessModifier.ToString().ToLower();
            var rt = RenderAbstractDataType(prop.DataType);
            var fieldNamePascal = prop.Name.ToProperCase().ToPascalCase();
            var fieldNameCamel = prop.Name.ToProperCase().ToCamelCase();

            WriteIndented($"private $_{fieldNameCamel};");
            WriteIndented(RenderDocBlock(
                null,
                new Dictionary<string, string>
                {
                    { "return", @"\something\E"}
                }
            ));
            /*
            private $_{fieldName};
            
            /**
             * @return \{projectName}\Entities\{dt}
             *
            public function get{fieldName}() : {rt}
            {
                return $this->_{fieldName};
            }

            /**
             * @param \{projectName}\Entities\{dt} $value
             * @return {className}
             *
            public function set{fieldName}($value) : {className}
            {
                $this->_{fieldName} = $value;

                return $this;
            }
             */

            //WriteIndented($"{visibility} {rt} {fieldName} {{ get; set; }}");
        }

        protected override string RenderAbstractVariableDefinition(AbstractVariableDefinition variable)
        {
            return $"{RenderAbstractDataType(variable.DataType)} {variable.Name}";
        }

        protected override void RenderApiClass(AbstractClass<ApiNode> ac)
        {
            
        }

        protected override void RenderEntityClass(AbstractClass<EntityNode> ac)
        {
            
        }

        protected override void RenderGenericClass(GenericClass genericClass)
        {
            
        }

        protected override void RenderGenericConstructor(GenericConstructor genericConstructor)
        {
            
        }

        protected override void RenderGenericFunction(GenericFunction genericFunction)
        {
            
        }

        protected override void RenderGenericProperty(GenericProperty prop)
        {
            
        }

        protected override void RenderGenericTryCatch(GenericTryCatch tryCatch)
        {
            
        }

        protected override string RenderGenericVariableDefinition(GenericVariableDefinition variableDefinition)
        {
            return "";
        }

        protected override List<string> RenderDocBlock(string description, Dictionary<string, string> paramValues)
        {
            var output = new List<string>();
            output.Add("/**");

            if (description != null)
            {
                output.Add($" * {description}");
                output.Add(" *");
            }
            
            foreach (var param in paramValues.Keys)
            {
                output.Add($" * @{param} {paramValues[param]}");
            }
            output.Add(" */");
            return output;
        }
    }
}