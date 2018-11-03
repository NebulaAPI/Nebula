using System;

namespace Nebula.SDK.Exceptions
{
    public class AlreadyInstalledException : AppException
    {
        public AlreadyInstalledException(string name) : base($"{name} is already installed.")
        {

        }
    }
}