using System;
using System.Linq;
using System.IO;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Nebula.Common.Data;
using Microsoft.EntityFrameworkCore;
using Nebula.SDK.Objects.Server;
using Nebula.SDK.Objects.Shared;
using Nebula.Core.Services.Server;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PluginController : ControllerBase
    {
        private RegistryService _registryService;
        private NebulaContext _nebulaContext;
        
        public PluginController(RegistryService registryService, NebulaContext context)
        {
            _registryService = registryService;
            _nebulaContext = context;
        }
        
        [HttpGet("{name}")]
        public Plugin Get(string name)
        {
            return _nebulaContext.Plugins
                .Include(p => p.Dependencies)
                .Include(p => p.Versions)
                .Include(p => p.UploadedBy)
                .FirstOrDefault(p => p.Name == name);
        }

        [HttpGet("search/{query}")]
        public List<Plugin> Search(string query)
        {
            return _nebulaContext.Plugins.Where(p => p.Name.Contains(query)).ToList();
        }
        
        [HttpPost]
        public Plugin Post([FromBody] string repoUrl)
        {
            var meta = _registryService.ImportPlugin(repoUrl);
            var versions = _registryService.GetPluginVersions(meta);
            var newPlugin = new Plugin {
                Name = meta.Name,
                Author = meta.Author,
                Description = meta.Description,
                LastUpdated = DateTime.Now,
                Verified = false,
                Active = false,
                RepositoryUrl = repoUrl,
                UploadedBy = new User { Id = Guid.NewGuid() },
                Dependencies = meta.Dependencies.Keys.Select(d => new PluginDependency {
                    Id = Guid.NewGuid(),
                    Name = d,
                    VersionPattern = meta.Dependencies[d]
                    }).ToArray(),
                Versions = versions.Keys.Select(v => new PluginVersion {
                    Id = Guid.NewGuid(),
                    Version = v,
                    CommitSha = versions[v],
                    DateAdded = DateTime.Now
                    }).ToArray()
            };

            _nebulaContext.Plugins.Add(newPlugin);
            _nebulaContext.SaveChanges();

            _registryService.CleanUpTemp(meta.TempFolder);

            return newPlugin;
        }
    }
}