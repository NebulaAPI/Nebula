using System;

namespace Nebula.SDK.Exceptions
{
    public class AlreadyInstalledException : Exception
    {
        public AlreadyInstalledException(string name) : base($"{name} is already installed.")
        {

        }
    }
}