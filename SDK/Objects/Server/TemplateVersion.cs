using System;
using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Objects.Server
{
    public class TemplateVersion : BaseTemplateVersion
    {
        public virtual ICollection<TemplateDependency> Dependencies { get; set; }
        
        public override void AddDependency(BaseDependency dep)
        {
            throw new NotImplementedException();
        }
    }
}