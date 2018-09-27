using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Nebula.Models;
using Nebula.Parser;
using Nebula.Services;
using Nebula.Util;
using System.Linq;
using System;
using Nebula.Compiler.Objects.Csharp;
using Nebula.Compiler.Objects;
using Nebula.Compiler.Abstracts;

namespace Nebula.Renderers
{
    public class CSharpRenderer : AbstractRenderer
    {
        public CSharpRenderer()
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

        protected override void RenderAbstractFunction(AbstractFunction function)
        {
            var visibility = function.AccessModifier.ToString().ToLower();
            var rt = RenderAbstractDataType(function.ReturnType);
            var args = string.Join(", ", function.Arguments.Select(a => RenderAbstractVariableDefinition(a)));

            WriteIndented($"{visibility} {rt} {function.Name}({args})");
            WriteIndented("{");
            IndentLevel++;

            IndentLevel--;
            WriteIndented("}");
        }

        protected override void RenderAbstractNamespace(AbstractNamespace ns)
        {
            foreach (var import in ns.Imports)
            {
                CurrentOutput.Add($"using {import};");
            }
            
            CurrentOutput.Add($"namespace {ns.Name}");
            CurrentOutput.Add("{");
        }

        protected override void RenderAbstractProperty(AbstractProperty prop)
        {
            
        }

        protected override string RenderAbstractVariableDefinition(AbstractVariableDefinition variable)
        {
            return $"{RenderAbstractDataType(variable.DataType)} {variable.Name}";
        }

        protected override void RenderApiClass(AbstractClass<ApiNode> ac)
        {
            RenderNode(ac.Namespace);
            IndentLevel++;
            WriteIndented($"public class {ac.Name}Client");
            WriteIndented("{");
            IndentLevel++;
            foreach (var extra in ac.TopOfClassExtra)
            {
                RenderNode(extra);
            }
            foreach (var prop in ac.Properties)
            {
                RenderNode(prop);
            }
            RenderNode(ac.Constructor);
            foreach (var func in ac.Functions)
            {
                RenderAbstractFunction(func);
            }
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
            foreach (var prop in genericClass.Properties)
            {
                RenderGenericProperty(prop);
            }
            RenderGenericConstructor(genericClass.Constructor);
            foreach (var func in genericClass.Functions)
            {
                RenderGenericFunction(func);
            }
            IndentLevel--;
            WriteIndented("}");
        }

        protected override void RenderGenericConstructor(GenericConstructor genericConstructor)
        {
            
        }

        protected override void RenderGenericFunction(GenericFunction genericFunction)
        {
            var visibility = genericFunction.AccessModifier.ToString().ToLower();
            var rt = genericFunction.ReturnType;
            var name = genericFunction.Name;
            var args = string.Join(", ", genericFunction.Arguments.Select(a => $"{a.DataTypeName} {a.Name}"));
            WriteIndented($"{visibility} {rt} {name}({args})");
            WriteIndented("{");
            IndentLevel++;
            foreach (var bodyLine in genericFunction.Body)
            {
                WriteIndented(bodyLine);
            }
            IndentLevel--;
            WriteIndented("}");
        }

        protected override void RenderGenericProperty(GenericProperty prop)
        {
            WriteIndented($"{prop.AccessModifier.ToString().ToLower()} {prop.DataTypeString} {prop.Name} {{ get; set; }}");
        }

        protected override void RenderGenericVariableDefinition(GenericVariableDefinition variableDefinition)
        {
            
        }
    }
}