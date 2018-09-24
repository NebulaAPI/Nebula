using System.IO;
using Nebula.Models;
using Nebula.Parser;
using System.Linq;
using System;

namespace Nebula.Renderers
{
    public abstract class AbstractRenderer
    {
        public abstract void Render(ProjectNode project, TemplateMeta templateMeta);

        public abstract void PrepareOutputDir(Project project, TemplateMeta templateMeta);

        protected abstract string ConvertTypeName(string inputType);

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
                        if (Enum.TryParse(typeof(AuthenticationMethod), kvNode.Value, true, out var enumVal))
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

        protected static void Copy(string sourceDirectory, string targetDirectory)
        {
            var diSource = new DirectoryInfo(sourceDirectory);
            var diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        protected static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (var fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (var diSourceSubDir in source.GetDirectories())
            {
                var nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}