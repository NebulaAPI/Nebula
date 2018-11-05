using System.Collections.Generic;
using Nebula.SDK.Objects;
using Nebula.SDK.Objects.Client;

namespace Nebula.SDK.Objects
{
    public class TemplateManifest
    {
        public List<Template> Templates {get; set; }

        public TemplateManifest AddTemplate(Template template)
        {
            if (Templates == null)
            {
                Templates = new List<Template> { template };
            }

            return this;
        }
    }
}