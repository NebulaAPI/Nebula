using System.Collections.Generic;
using Nebula.SDK.Objects.Shared;

namespace Nebula.SDK.Objects.Server
{
    public class Template : BaseTemplate
    {
        public ICollection<TemplateVersion> Versions { get; set; }
    }
}