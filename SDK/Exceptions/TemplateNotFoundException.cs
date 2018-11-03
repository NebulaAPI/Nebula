using System;

namespace Nebula.SDK.Exceptions
{
    public class TemplateNotFoundException : AppException
    {
        public TemplateNotFoundException(string name) : base($"Template {name} not found.")
        {

        }
    }
}