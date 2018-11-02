using System;

namespace Nebula.SDK.Exceptions
{
    public class ValidationException : AppException
    {
        public ValidationException(string message) : base(message)
        {
            
        }
    }
}