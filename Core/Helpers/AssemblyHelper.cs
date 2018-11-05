using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nebula.Core.Helpers
{
    public static class AssemblyHelper
    {
        public static T SearchForType<T>(this Assembly assembly)
        {
            Func<Type, T> createFunc = (t) => (T)Activator.CreateInstance(t);
            
            var searchActions = new List<Func<Type[], T>> {
                (a) => a.Where(t => t.GetInterfaces().Any(i => i.Name.Contains(typeof(T).Name)))
                    .Select(t => createFunc(t))
                    .FirstOrDefault(),
                (a) => a.Where(t => t.BaseType == typeof(T))
                    .Select(t => createFunc(t))
                    .FirstOrDefault(),
                (a) => a.Where(t => t == typeof(T))
                    .Select(t => createFunc(t))
                    .FirstOrDefault()
            };

            var types = assembly.GetTypes();

            return searchActions.Select(a => a(types)).Where(t => t != null).FirstOrDefault();
        }
    }
}