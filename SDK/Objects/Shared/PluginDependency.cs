using System;

namespace Nebula.SDK.Objects.Shared
{
    public class PluginDependency
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string VersionPattern { get; set; }
    }
}