using System;
using System.Collections.Generic;
using System.Linq;
using Nebula.SDK.Compiler.Objects;
using Nebula.SDK.Plugin;
using Nebula.SDK.Objects;

namespace Nebula.SDK.Compiler.Abstracts
{
    public abstract class AbstractCompiler
    {
        public Project Project { get; set; }

        public ProjectNode ProjectNode { get; set; }

        public TemplateMeta TemplateMeta { get; set; }

        public Dictionary<ApiNode, ApiConfig> ApiConfig { get; set; }

        public ICompilerExtension CompilerExtension { get; set; }

        public abstract List<OutputFile> Compile();

        public void Init(Project project, ProjectNode projectNode, TemplateMeta meta, ICompilerExtension compilerExtension)
        {
            Project = project;
            ProjectNode = projectNode;
            TemplateMeta = meta;
            CompilerExtension = compilerExtension;
            ApiConfig = new Dictionary<ApiNode, ApiConfig>();
            BuildApiConfigs();
        }

        private void BuildApiConfigs()
        {
            var apis = ProjectNode.SearchByType<ApiNode>();
            foreach (var api in apis)
            {
                ApiConfig.Add(api, GetApiConfig(api));
            }
        }

        protected List<C> GetClassesByType<N, C>(AbstractNamespace ns, AbstractCompiler compiler) 
            where N : MainObjectNode
            where C : AbstractClass<N>, new()
        {
            return ProjectNode
                .SearchByType<N>()
                .Select(n => new C().Build<C>(ns, n, compiler)).ToList();
        }

        protected ApiConfig GetApiConfig(ApiNode node)
        {
            var config = new ApiConfig { AuthMethod = AuthenticationMethod.NoAuthentication };
            var configNode = node.SearchByType<ConfigNode>().FirstOrDefault();
            if (configNode == null)
            {
                throw new System.Exception($"Could not find API configuration for {node.Name}");
            }

            foreach (var configProp in config.GetType().GetProperties())
            {
                // find KV pair in config node that matches this property
                var kvNode = configNode.SearchByType<KeyValueNode>().Where(n => n.Key.ToLower() == configProp.Name.ToLower()).FirstOrDefault();
                if (kvNode != null)
                {
                    if (typeof(AuthenticationMethod).IsAssignableFrom(configProp.PropertyType))
                    {
                        if (Enum.TryParse<AuthenticationMethod>(kvNode.Value, true, out var enumVal))
                        {
                            configProp.SetValue(config, enumVal);
                        }
                        else
                        {
                            throw new Exception($"Invalid value for {configProp.Name}");
                        }
                    }
                    else
                    {
                        configProp.SetValue(config, kvNode.Value);
                    }
                    
                }
            }

            return config;
        }
    }
}