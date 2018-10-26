using System;
using System.Collections.Generic;

namespace Nebula.Common.Data.Models
{
    public class Template
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public User Author { get; set; }
        public Guid AuthorId { get; set; }
        public string Description { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime Published { get; set; }
        public bool Verified { get; set; }
        public bool Active { get; set; }

        public ICollection<TemplateVersion> Versions { get; set; }
    }
}