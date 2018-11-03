using System;
using System.Collections.Generic;
using System.Linq;
using Nebula.SDK.Objects;

namespace Nebula.Core.Parser
{
    public interface IProjectValidator
    {
        void Validate(ProjectNode projectNode);
    }
    
    /// <summary>
    /// This class is responsible for checking that specified data types exist
    /// in the project, either because they are the default included types, or
    /// a specified custom entity type
    /// 
    /// First we have to find all the entity nodes and collect those types
    /// </summary>
    public class ProjectValidator : IProjectValidator
    {
        private List<EntityNode> _customEntities;
        private List<string> _builtInTypes;
        private List<string> _allTypes;

        public ProjectValidator()
        {
            _builtInTypes = new List<string> {
                "string", "boolean", "integer", "float", "double", "char", "array", "datetime"
            };
            _allTypes = new List<string>(_builtInTypes);
        }

        public void Validate(ProjectNode projectNode)
        {
            CollectCustomEntities(projectNode);
            ValidateDataNodes(projectNode);
            ValidateApiConfig(projectNode);
            ValidateFunctionDocs(projectNode);
        }

        private bool IsValidType(string typeName)
        {
            return _allTypes.Contains(typeName);
        }

        private void ValidateFunctionDocs(ProjectNode projectNode)
        {
            var funcs = projectNode.SearchByType<FunctionNode>();
            foreach (var f in funcs)
            {
                if (!f.Docs.Any(d => d.Key == "description"))
                {
                    throw new Exception($"Missing 'description' doc on function: {f.Name}");
                }
                
                if (!f.Docs.Any(d => d.Key == "return"))
                {
                    throw new Exception($"Missing 'return' doc on function: {f.Name}");
                }

                foreach (var arg in f.Args)
                {
                    if (!f.Docs.Any(d => d.Key == arg.Name))
                    {
                        throw new Exception($"Missing documentation for function parameter: {arg.Name}");
                    }
                }
            }
        }

        private void ValidateApiConfig(ProjectNode projectNode)
        {
            var apiConfigNodes = projectNode.SearchByType<ConfigNode>();
            var realConfigProps = typeof(ApiConfig).GetProperties();
            foreach (var config in apiConfigNodes)
            {
                foreach (var kv in config.Nodes)
                {
                    if (!realConfigProps.Any(p => p.Name.ToLower() == kv.Key.ToLower()))
                    {
                        throw new Exception($"Unknown config value: {kv.Key}");    
                    }
                }
            }
        }

        private void ValidateDataNodes(ProjectNode projectNode)
        {
            Action<string> throwInvalid = (dt) => throw new System.Exception("Unknown data type " + dt);
            var dataNodes = projectNode.SearchByType<DataTypeNode>();
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

        private void CollectCustomEntities(ProjectNode projectNode)
        {
            _customEntities = new List<EntityNode>();
            foreach (var module in projectNode.Modules)
            {
                _customEntities.AddRange(module.Elements.AsEnumerable().OfType<EntityNode>());
            }
            _allTypes.AddRange(_customEntities.Select(e => e.Name));
        }
    }
}