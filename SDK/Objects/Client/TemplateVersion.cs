using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Objects.Client
{
    public class TemplateVersion : BaseTemplateVersion
    {
        public virtual List<TemplateDependency> Dependencies { get; set; }
        
        public override void AddDependency(BaseDependency dep)
        {
            throw new System.NotImplementedException();
        }
    }
}