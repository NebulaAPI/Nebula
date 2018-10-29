using System.IO;
using LibGit2Sharp;
using Nebula.SDK.Objects;
using Newtonsoft.Json;

namespace Nebula.Common.API.Services
{
    public class RegistryService
    {
        public PluginMeta Import(string repoUrl)
        {
            var tmpPath = Path.GetRandomFileName();
            tmpPath = Path.Combine(NebulaConfig.TempDirectory, tmpPath);
            var metaFile = Path.Combine(tmpPath, "nebula-meta.json");
            var repo = Repository.Clone(repoUrl, tmpPath);
            if (!File.Exists(metaFile))
            {
                throw new System.Exception($"Repository does not contain nebula-meta.json: {metaFile}");
            }
            var meta = JsonConvert.DeserializeObject<PluginMeta>(File.ReadAllText(metaFile));
            return meta;
        }
    }
}