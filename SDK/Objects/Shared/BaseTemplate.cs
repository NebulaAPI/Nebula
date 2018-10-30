using System;

namespace Nebula.SDK.Objects.Shared
{
    public class BaseTemplate
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
        public TemplateLanguagePlugin LanguagePlugin { get; set; }
    }
}