using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nebula.Common.Data;
using Nebula.Core.Services.Server;
using Nebula.SDK.Objects.Server;
using Nebula.SDK.Objects.Shared;

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
            _registryService.Db = _nebulaContext;

            
        }

        [AllowAnonymous]
        [HttpGet("{name}")]
        public Template Get(string name)
        {
            return _nebulaContext.Templates
                .Include(p => p.Versions)
                    .ThenInclude(version => version.Dependencies)
                .Include(p => p.UploadedBy)
                .FirstOrDefault(p => p.Name == name);
        }

        [AllowAnonymous]
        [HttpGet("search/{query}")]
        public List<Template> Search(string query)
        {
            return _nebulaContext.Templates.Where(p => p.Name.Contains(query)).ToList();
        }

        [Authorize]
        [HttpPost]
        public Template Post([FromBody] string repoUrl)
        {
            var user = _nebulaContext.Users.FirstOrDefault(u => u.Username == User.Identity.Name);
            if (user != null)
            {
                _registryService.User = user;
            }
            
            var template = _registryService.ImportTemplate(repoUrl);

            try
            {
                _registryService.VerifyTemplate(template);
                _nebulaContext.Templates.Add(template);
                _nebulaContext.SaveChanges();
            }
            catch (System.Exception)
            {
                throw;
            }

            return template;
        }
    }
}