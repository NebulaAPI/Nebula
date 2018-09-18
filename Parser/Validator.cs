using System;
using System.Collections.Generic;
using System.Linq;

namespace Nebula.Parser
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

        private List<string> BuiltInTypes { get; set; }

        private List<string> AllTypes { get; set; }
        public Validator(ProjectNode projectNode)
        {
            Node = projectNode;
            BuiltInTypes = new List<string> {
                "string", "boolean", "integer", "float", "double", "char", "array", "datetime"
            };
            AllTypes = new List<string>(BuiltInTypes);
        }

        public void Validate()
        {
            CollectCustomEntities();
            ValidateDataNodes();
        }

        private bool IsValidType(string typeName)
        {
            return AllTypes.Contains(typeName);
        }

        private void ValidateDataNodes()
        {
            Action<string> throwInvalid = (dt) => throw new System.Exception("Unknown data type " + dt);
            var dataNodes = Node.SearchByType<DataTypeNode>();
            foreach (var dataNode in dataNodes)
            {
                if (IsValidType(dataNode.Name))
                {
                    if (dataNode.Generic)
                    {
                        if (IsValidType(dataNode.GenericType))
                        {
                            continue;
                        }
                        else
                        {
                            throwInvalid(dataNode.GenericType);
                        }
                    }

                    continue;
                }

                throwInvalid(dataNode.Name);
            }
        }

        private void CollectCustomEntities()
        {
            CustomEntities = new List<EntityNode>();
            foreach (var module in Node.Modules)
            {
                CustomEntities.AddRange(module.Elements.AsEnumerable().OfType<EntityNode>());
            }
            AllTypes.AddRange(CustomEntities.Select(e => e.Name));
        }
    }
}