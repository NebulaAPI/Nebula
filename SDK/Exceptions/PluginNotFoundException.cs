using System;

namespace Nebula.SDK.Exceptions
{
    public class PluginNotFoundException : AppException
    {
        public PluginNotFoundException(string name) : base($"Plugin {name} not found.")
        {

        }
    }
}