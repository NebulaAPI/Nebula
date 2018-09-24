using Nebula.Models;
using Nebula.Parser;

namespace Nebula.Renderers
{
    public class JavaRenderer : AbstractRenderer
    {
        public override void PrepareOutputDir(Project project, TemplateMeta templateMeta)
        {
            throw new System.NotImplementedException();
        }

        public override void Render(ProjectNode project, TemplateMeta templateMeta)
        {
            throw new System.NotImplementedException();
        }

        protected override string ConvertTypeName(string inputType)
        {
            throw new System.NotImplementedException();
        }
    }
}