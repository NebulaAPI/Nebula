using System.IO;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Nebula.Common.API.Services;
using Nebula.SDK.Objects;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class PluginController : ControllerBase
    {
        private RegistryService _registryService;
        
        public PluginController(RegistryService registryService)
        {
            _registryService = registryService;
        }
        
        [HttpGet("{name}")]
        public PluginMeta Get(string name)
        {
            return new PluginMeta { Name = name };
        }
        
        [HttpPost]
        public PluginMeta Post([FromBody] string repoUrl)
        {
            return _registryService.Import(repoUrl);
        }
    }
}