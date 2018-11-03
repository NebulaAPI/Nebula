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
using Microsoft.AspNetCore.Authorization;

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
        
        [AllowAnonymous]
        [HttpGet("{name}")]
        public Plugin Get(string name)
        {
            return _nebulaContext.Plugins
                .Include(p => p.Versions)
                    .ThenInclude(version => version.Dependencies)
                .Include(p => p.UploadedBy)
                .FirstOrDefault(p => p.Name == name);
        }

        [AllowAnonymous]
        [HttpGet("search/{query}")]
        public List<Plugin> Search(string query)
        {
            return _nebulaContext.Plugins.Where(p => p.Name.Contains(query)).ToList();
        }
        
        [HttpPost]
        [Authorize]
        public Plugin Post([FromBody] string repoUrl)
        {
            var user = _nebulaContext.Users.FirstOrDefault(u => u.Id == Guid.Parse(User.Identity.Name));
            if (user != null)
            {
                _registryService.User = user;
            }
            
            var plugin = _registryService.ImportPlugin(repoUrl);

            _nebulaContext.Plugins.Add(plugin);
            _nebulaContext.SaveChanges();

            return plugin;
        }
    }
}