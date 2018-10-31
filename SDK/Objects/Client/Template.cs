using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Objects.Client
{
    public class Template : BaseTemplate
    {
        public List<TemplateVersion> Versions { get; set; }
    }
}