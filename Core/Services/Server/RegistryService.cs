using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Nebula.Common.Data;
using Nebula.SDK.Exceptions;
using Nebula.SDK.Objects;
using Nebula.SDK.Objects.Server;
using Nebula.SDK.Objects.Shared;
using Nebula.SDK.Util;
using Newtonsoft.Json;
using SemVer;

namespace Nebula.Core.Services.Server
{
    public class RegistryService
    {
        public NebulaContext Db { get; set; }

        public User User { get; set; }
        
        public Plugin ImportPlugin(string repoUrl)
        {
            using (var repo = ImportObject<PluginMeta>(repoUrl))
            {
                var meta = GetMeta<PluginMeta>(repo.Info.WorkingDirectory);
                var plugin = new Plugin
                {
                    Name = meta.Name,
                    Author = meta.Author,
                    Description = meta.Description,
                    LastUpdated = DateTime.Now,
                    Published = DateTime.Now,
                    UploadedBy = User,
                    UploadedById = User.Id,
                    RepositoryUrl = repoUrl
                };

                plugin.Versions = GetVersionObjects<PluginVersion>(repo, meta);

                return plugin;
            }
        }

        public Template ImportTemplate(string repoUrl)
        {
            using (var repo = ImportObject<TemplateMeta>(repoUrl))
            {
                var meta = GetMeta<TemplateMeta>(repo.Info.WorkingDirectory);
                var template = new Template
                {
                    Name = meta.Name,
                    Author = meta.Author,
                    Description = meta.Description,
                    LastUpdated = DateTime.Now,
                    Published = DateTime.Now,
                    UploadedBy = User,
                    RepositoryUrl = repoUrl
                };

                template.Versions = GetVersionObjects<TemplateVersion>(repo, meta);
                template.LanguagePlugin = new TemplateLanguagePlugin {
                    Name = meta.LanguagePlugin.Name,
                    Version = meta.LanguagePlugin.Version
                };

                return template;
            }
        }

        private List<T> GetVersionObjects<T>(Repository repository, MetaObject meta) where T: BaseVersion, new()
        {
            var result = new List<T>();
            var versions = GetObjectVersions(meta, repository);
            foreach (var ver in versions.Keys)
            {
                var newVersion = new T
                {
                    Version = ver,
                    Verified = false,
                    Active = false,
                    CommitSha = versions[ver]
                };
                
                Reference reference = null;

                var tag = repository.Tags[ver];
                if (tag == null)
                {
                    var master = repository.Branches["master"];
                    reference = master.Reference;
                }
                else
                {
                    reference = tag.Reference;
                }
                var commit = repository.Lookup<Commit>(reference.TargetIdentifier);
                var tree = commit.Tree;
                repository.Reset(ResetMode.Hard, commit);
                repository.Checkout(tree, null, new CheckoutOptions());
                var versionMeta = GetMeta<TemplateMeta>(repository.Info.WorkingDirectory);
                
                foreach (var dep in versionMeta.Dependencies.Keys)
                {
                    newVersion.AddDependency(new BaseDependency { Name = dep, VersionPattern = versionMeta.Dependencies[dep]});
                }

                result.Add(newVersion);
            }
            return result;
        }

        private T GetMeta<T>(string tmpPath) where T : MetaObject
        {
            var metaFile = Path.Combine(tmpPath, "nebula-meta.json");
            if (!File.Exists(metaFile))
            {
                throw new System.Exception($"Repository does not contain nebula-meta.json: {metaFile}");
            }
            var meta = JsonConvert.DeserializeObject<T>(File.ReadAllText(metaFile));
            if (meta.Dependencies == null)
            {
                meta.Dependencies = new Dictionary<string, string>();
            }
            meta.TempFolder = tmpPath;
            return meta;
        }

        private Repository ImportObject<T>(string repoUrl) where T : MetaObject
        {
            var tmpPath = Path.GetRandomFileName();
            tmpPath = Path.Combine(NebulaConfig.TempDirectory, tmpPath);
            
            Repository.Clone(repoUrl, tmpPath);
            var repo = new Repository(tmpPath);
            
            return repo;
        }

        public void VerifyPlugin(string pluginName, string requestedVersion)
        {
            var plugin = Db.QueryPlugins(p => p.Name == pluginName).FirstOrDefault();
            if (plugin == null)
            {
                throw new ValidationException($"Plugin {pluginName} not found");
            }

            try
            {
                var requestedVersionRange = new Range(requestedVersion);
                var foundGoodVersion = false;
                foreach (var pluginVersion in plugin.Versions)
                {
                    var ver = new SemVer.Version(pluginVersion.Version);
                    if (requestedVersionRange.IsSatisfied(ver))
                    {
                        foundGoodVersion = true;
                        break;
                    }
                }

                if (!foundGoodVersion)
                {
                    throw new ValidationException($"Could not find matching version for plugin {pluginName}. Requested version: {requestedVersion}");
                }
            }
            catch (ArgumentException ae)
            {
                throw new ValidationException($"Error processing requested version: {requestedVersion}: {ae.Message}");
            }
        }

        public void VerifyTemplate(Template template)
        {
            // 1. verify that the specified language plugin exists
            VerifyPlugin(template.LanguagePlugin.Name, template.LanguagePlugin.Version);
            
            // 2. verify that the specified dependencies exist
            //template.Dependencies.ToList().ForEach(d => VerifyPlugin(d.Name, d.VersionPattern));
        }

        public Dictionary<string, string> GetObjectVersions(MetaObject meta, Repository repo)
        {
            var master = repo.Branches["master"];
            if (master == null)
            {
                throw new ValidationException($"Could not find any version tags or master branch for {meta.Name}");
            }
            var versions = repo.Tags.ToDictionary(t => t.FriendlyName, t => t.Reference.TargetIdentifier);

            versions.Add("master", master.Reference.TargetIdentifier);

            return versions;
        }

        public void CleanUpTemp(string tmpPath)
        {
            Directory.Delete(tmpPath, true);
        }
    }
}