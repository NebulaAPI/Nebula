using System;

namespace Nebula.Compiler.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class EventHandlerAttribute : Attribute
    {
        public Events.Events Event { get; set; }

        public EventHandlerAttribute()
        {
            
        }
    }
}