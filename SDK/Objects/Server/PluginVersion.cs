using System;
using System.Collections.Generic;

namespace Nebula.SDK.Objects.Shared
{
    public class PluginVersion : BasePluginVersion
    {
        public virtual ICollection<BaseDependency> Dependencies { get; set; }

        public override void AddDependency(BaseDependency dep)
        {
            if (Dependencies == null)
            {
                Dependencies = new List<BaseDependency>();
                Dependencies.Add(dep);
            }
        }
    }
}