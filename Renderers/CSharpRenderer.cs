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

namespace Nebula.Renderers
{
    public class CSharpRenderer : AbstractRenderer
    {
        private string DestinationDirectory { get; set; }

        private Project Project { get; set; }

        private TemplateMeta Meta { get; set; }
        
        public override void Render(ProjectNode project, TemplateMeta templateMeta)
        {
            Meta = templateMeta;
            
            // first we get the list of entity nodes and render those
            var entityNodes = project.SearchByType<EntityNode>();
            RenderEntities(entityNodes);

            var apiNodes = project.SearchByType<ApiNode>();
            RenderApis(apiNodes);
        }

        private void RenderApis(List<ApiNode> apis)
        {
            foreach (var api in apis)
            {
                var apiConfig = GetApiConfig(api);
                RenderApi(api, apiConfig);
            }
        }

        private void RenderApi(ApiNode api, ApiConfig config)
        {
            var output = $"using System.Collections.Generic;\nusing RestSharp;\nusing {Project.Name}.{Meta.EntityLocation};\n\n";
            output += $"namespace {Project.Name}.{Meta.ClientLocation}\n{{";
            output += $"\n\tpublic class {api.Name}Client\n\t{{\n";
            output += $"\t\tprivate RestClient Client {{ get; set; }}\n\n";
            output += $"\t\tpublic {api.Name}Client()\n\t\t{{";
            output += $"\n\t\t\tClient = new RestClient(\"{config.Host}\");";
            output += "\n\t\t}";
            foreach (var f in api.SearchByType<FunctionNode>())
            {
                output += RenderFunction(f, config);
            }
            output += "\n\t}\n}\n";
            var outputFileName = Path.Join(DestinationDirectory, Meta.ClientLocation, $"{api.Name}.cs");
            File.WriteAllText(outputFileName, output);
        }

        private string RenderType(DataTypeNode node)
        {
            if (node.Generic && node.Name == "array")
            {
                return $"List<{node.GenericType}>";
            }

            return $"{ConvertTypeName(node.Name)}";
        }

        private List<string> RenderArguments(List<ArgumentNode> arguments)
        {
            var output = new List<string>();

            foreach (var arg in arguments)
            {
                output.Add($"{RenderType(arg.ArgType)} {arg.Name}");
            }

            return output;
        }

        private string GetHttpMethod(TokenType functionType)
        {
            switch (functionType)
            {
                case TokenType.GetFunction: return "Method.GET";
                case TokenType.PostFunction: return "Method.POST";
                case TokenType.PutFunction: return "Method.PUT";
                case TokenType.DeleteFunction: return "Method.DELETE";
                default: throw new System.Exception("Unknown function method type");
            }
        }

        private string RenderUrlSegment(string url, List<ArgumentNode> args)
        {
            var output = "";
            // look in the URL for {variable} strings and then try and find a matching function argument
            // if we find it, generate the appropriate request.AddUrlSegment call
            // for any argument that is not part of the URL, send that as a parameter
            var regex = new Regex(@"({[a-z]+})", RegexOptions.IgnoreCase);
            var matches = regex.Matches(url);
            var usedArgs = new List<ArgumentNode>();
            foreach (Match m in matches)
            {
                var parameterName = m.Value.Replace("{", "").Replace("}", "");
                var matchingArg = args.Where(a => a.Name == parameterName).FirstOrDefault();
                if (matchingArg == null)
                {
                    throw new Exception("No matching argument for URL parameter: " + parameterName);
                }
                usedArgs.Add(matchingArg);
                output += $"\n\t\t\trequest.AddUrlSegment(\"{parameterName}\", {matchingArg.Name});";
            }

            var unusedArgs = args.Where(a => !usedArgs.Contains(a));
            foreach (var arg in unusedArgs)
            {
                output += $"\n\t\t\trequest.AddParameter(\"{arg.Name}\", {arg.Name});";
            }

            return output;
        }

        private string RenderFunction(FunctionNode function, ApiConfig config)
        {
            var prefix = config.Prefix;
            var output = $"\n\n\t\tpublic {RenderType(function.ReturnType)} {function.Name.ToProperCase().ToPascalCase()}(";
            var args = RenderArguments(function.Args);
            output += string.Join(", ", args);
            output += ")\n\t\t{";
            var method = GetHttpMethod(function.Method);
            output += $"\n\t\t\tvar request = new RestRequest(\"{prefix}{function.Url}\", {method});";
            //output += $"\n\t\t\t"
            output += RenderUrlSegment(function.Url, function.Args);
            var returnType = RenderType(function.ReturnType);
            output += $"\n\t\t\tvar response = Client.Execute<{returnType}>(request);";
            output += "\n\t\t\treturn response.Data;";
            output += "\n\t\t}";
            return output;
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

        private void RenderEntities(List<EntityNode> entities)
        {
            foreach (var entity in entities)
            {
                var output = $"using System;\nusing System.Collections.Generic;\n\nnamespace {Project.Name}.{Meta.EntityLocation}\n{{";
                output += $"\n\tpublic class {entity.Name}\n\t{{\n";
                foreach (var field in entity.Fields)
                {
                    output += $"\t\t{RenderField(field)}\n\n";
                }
                output += "\t}\n}\n";
                var outputFileName = Path.Join(DestinationDirectory, Meta.EntityLocation, $"{entity.Name}.cs");
                File.WriteAllText(outputFileName, output);
            }
        }

        private string RenderField(ArgumentNode node)
        {
            var fieldName = node.Name.ToProperCase().ToPascalCase();
            var typeName = ConvertTypeName(node.ArgType.Name);

            if (node.ArgType.Generic && node.ArgType.Name == "array")
            {
                return $"public List<{node.ArgType.GenericType}> {fieldName} {{ get; set; }}";
            }

            return $"public {typeName} {fieldName} {{ get; set; }}";
        }

        public override void PrepareOutputDir(Project project, TemplateMeta templateMeta)
        {
            // here we need to copy the template folder to the output directory
            // and customize the template
            var sourceTemplatePath = Path.Join(project.TemplateDirectory, templateMeta.TemplateData.Name);
            var destTemplatePath = Path.Join(project.OutputDirectory, $"{project.Name}-csharp");

            DestinationDirectory = destTemplatePath;
            Project = project;
            
            Directory.Delete(destTemplatePath, true);
            Copy(sourceTemplatePath, destTemplatePath);

            var ts = new TemplateService(project, null);
            ts.CustomizeTemplate(destTemplatePath, templateMeta.TemplateData.Name);
        }
    }
}