using System;
using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Objects.Client
{
    public class PluginVersion : BasePluginVersion
    {
        public virtual List<PluginDependency> Dependencies { get; set; }

        public override void AddDependency(BaseDependency dep)
        {
            throw new NotImplementedException();
        }
    }
}