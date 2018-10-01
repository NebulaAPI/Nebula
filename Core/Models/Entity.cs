using System.Collections.Generic;

namespace CLI.Models
{
    public class Entity
    {
        public string Name { get; set; }

        public List<EntityField> Fields { get; set; }

        public Entity()
        {
            Fields = new List<EntityField>();
        }
    }
}