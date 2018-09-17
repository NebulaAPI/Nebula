using System.Collections.Generic;

namespace Nebula.Models
{
    public class Project
    {
        public string Name { get; set; }
        public List<Author> Authors { get; set; }
        public string Version { get; set; }
        public string Manifest { get; set; }
        public List<LibraryTemplate> Templates { get; set; }
        public string SourceDirectory { get; set; }

        public Project()
        {
            Authors = new List<Author>();
            Templates = new List<LibraryTemplate>();
        }

    }
}