using System;
using Newtonsoft.Json;

namespace Nebula.SDK.Objects.Shared
{
    public class BaseRegistryObject
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime Published { get; set; }
        [JsonIgnore] public User UploadedBy { get; set; }
        public Guid UploadedById { get; set; }
        public string RepositoryUrl { get; set; }
    }
}