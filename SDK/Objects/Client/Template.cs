using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;
using Nebula.SDK.Interfaces;

namespace Nebula.SDK.Objects.Client
{
    public class Template : BaseTemplate
    {
        public List<TemplateVersion> Versions { get; set; }

    }
}