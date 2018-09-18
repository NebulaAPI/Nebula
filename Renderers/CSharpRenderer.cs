using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Nebula.Models;
using Nebula.Parser;
using Nebula.Services;

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
        }

        protected override string ConvertTypeName(string inputType)
        {
            switch (inputType)
            {
                case "integer": return "int";
                case "boolean": return "bool";
                default: return inputType;
            }
        }

        private void RenderEntities(List<EntityNode> entities)
        {
            foreach (var entity in entities)
            {
                var output = $"using System.Collections.Generic;\n\nnamespace {Project.Name}.{Meta.EntityLocation}\n{{";
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
            var fieldName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(node.Name);
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