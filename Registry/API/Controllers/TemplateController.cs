using Microsoft.AspNetCore.Mvc;
using Nebula.Common.Data;
using Nebula.Core.Services.Server;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        private RegistryService _registryService;
        private NebulaContext _nebulaContext;
        
        public TemplateController(RegistryService registryService, NebulaContext context)
        {
            _registryService = registryService;
            _nebulaContext = context;
        }
    }
}