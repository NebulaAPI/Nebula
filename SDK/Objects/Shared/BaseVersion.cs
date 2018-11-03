using System;

namespace Nebula.SDK.Objects.Shared
{
    public abstract class BaseVersion
    {
        public Guid Id { get; set; }
        public string Version { get; set; }
        public string CommitSha { get; set; }
        public DateTime DateAdded { get; set; }
        public bool Verified { get; set; }
        public bool Active { get; set; }

        public abstract void AddDependency(BaseDependency dep);
    }
}