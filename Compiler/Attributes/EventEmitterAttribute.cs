using System;
using System.Collections.Generic;

namespace Nebula.Compiler.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventEmitterAttribute : Attribute
    {
        public List<Events.Events> Events { get; set; }

        public EventEmitterAttribute()
        {
            
        }
    }
}