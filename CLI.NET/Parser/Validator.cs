using System.Collections.Generic;
using System.Linq;

namespace CLI.NET.Parser
{
    /// <summary>
    /// This class is responsible for checking that specified data types exist
    /// in the project, either because they are the default included types, or
    /// a specified custom entity type
    /// 
    /// First we have to find all the entity nodes and collect those types
    /// </summary>
    public class Validator
    {
        private ProjectNode Node { get; set; }

        private List<EntityNode> CustomEntities { get; set; }
        public Validator(ProjectNode projectNode)
        {
            Node = projectNode;
        }

        public void Validate()
        {
            CollectCustomEntities();
        }

        private void CollectCustomEntities()
        {
            CustomEntities = new List<EntityNode>();
            foreach (var module in Node.Modules)
            {
                CustomEntities.AddRange(module.Elements.AsEnumerable().OfType<EntityNode>());
            }
        }
    }
}