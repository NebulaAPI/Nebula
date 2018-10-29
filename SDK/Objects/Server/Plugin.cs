using System;
using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Objects.Server
{
    public class Plugin
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime Published { get; set; }
        public bool Verified { get; set; }
        public bool Active { get; set; }
        public User UploadedBy { get; set; }
        public Guid UploadedById { get; set; }
        public string RepositoryUrl { get; set; }

        public virtual ICollection<PluginVersion> Versions { get; set; }
        public virtual ICollection<PluginDependency> Dependencies { get; set; }
    }
}